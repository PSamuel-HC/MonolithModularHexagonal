using AutoMapper;
using MassTransit;
using MediatR;
using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Application.Ports;
using MyModularStore.Orders.Domain.Entities;
using MyModularStore.Shared.Events;

namespace MyModularStore.Orders.Application.Commands
{
    public class CreateOrderCommandHandler(
        IOrderRepository repository,
        IMapper mapper,
        IPublishEndpoint publishEndpoint
        ) : IRequestHandler<CreateOrderCommand, OrderReadDto>
    {
        public async Task<OrderReadDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            //await validator.ValidateAndThrowAsync(request.dto, cancellationToken);

            var order = mapper.Map<Order>(request.dto);

            await repository.AddAsync(order, cancellationToken);

            await publishEndpoint.Publish(new OrderPlacedEvent
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                OrderNumber = order.OrderNumber,
                PlacedAt = DateTime.UtcNow
            }, cancellationToken);

            //logger.LogInformation(
            //    "Order {OrderId} ({OrderNumber}) created via CQRS handler",
            //    order.Id, order.OrderNumber);

            return mapper.Map<OrderReadDto>(order);
        }
    }
}
