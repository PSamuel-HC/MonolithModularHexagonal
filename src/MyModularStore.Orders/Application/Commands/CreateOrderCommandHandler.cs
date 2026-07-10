using AutoMapper;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Application.Ports;
using MyModularStore.Orders.Domain.Entities;
using MyModularStore.Shared.Events;

namespace MyModularStore.Orders.Application.Commands
{
    public class CreateOrderCommandHandler(
        IOrderRepository repository,
        IMapper mapper,
        IPublishEndpoint publishEndpoint,
        IDistributedCache cache) : IRequestHandler<CreateOrderCommand, OrderReadDto>
    {
        public async Task<OrderReadDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            Order order = mapper.Map<Order>(request.dto);

            await repository.AddAsync(order, cancellationToken);

            await publishEndpoint.Publish(new OrderPlacedEvent
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                OrderNumber = order.OrderNumber,
                PlacedAt = DateTime.UtcNow
            }, cancellationToken);

            await cache.RemoveAsync("orders:all", cancellationToken);

            return mapper.Map<OrderReadDto>(order);
        }
    }
}
