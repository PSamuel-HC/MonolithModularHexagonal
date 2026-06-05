using AutoMapper;
using FluentValidation;
using MassTransit;
using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Application.Ports;
using MyModularStore.Orders.Application.Validators;
using MyModularStore.Orders.Domain.Entities;
using MyModularStore.Shared.Commands;
using MyModularStore.Shared.Contracts;
using MyModularStore.Shared.Events;
using MyModularStore.Shared.Exceptions;

namespace MyModularStore.Orders.Application.Services
{
    public class OrderService(
        IOrderRepository repository,
        IMapper mapper,
        OrderCreateDtoValidators createValidator,
        OrderUpdateDtoValidators updateValidator,
        ICustomerContract customerContract,
        //ISendEndpointProvider sendEndpointProvider,
        IPublishEndpoint publishEnpoint
    ) : IOrderModule
    {
        public async Task<IEnumerable<OrderReadDto>> GetAllAsync(CancellationToken ct = default)
        {
            var orders = await repository.GetAllAsync(ct);
            return mapper.Map<IEnumerable<OrderReadDto>>(orders);
        }

        public async Task<OrderReadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var order = await repository.GetByIdAsync(id, ct);
            return order is null ? null : mapper.Map<OrderReadDto>(order);
        }

        public async Task<OrderReadDto> CreateAsync(OrderCreateDto dto, CancellationToken ct = default)
        {
            await createValidator.ValidateAndThrowAsync(dto, ct);

            var customerExists = await customerContract.ExistsAsync(dto.CustomerId!.Value);
            if (!customerExists)
                throw new NotFoundException($"Customer with id {dto.CustomerId} not found.");

            var order = mapper.Map<Order>(dto);
            await repository.AddAsync(order);

            // Queue — destination resolved from EndpointConvention.Map<FulfillOrderCommand> in Program.cs
            //await sendEndpointProvider.Send(new FulfillOrderCommand
            //{
            //    OrderId = order.Id,
            //    CustomerId = order.CustomerId,
            //    OrderNumber = order.OrderNumber
            //});

            // Topic — fan-out to all subscribers
            await publishEnpoint.Publish(new OrderPlacedEvent
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                OrderNumber = order.OrderNumber,
                PlacedAt = DateTime.UtcNow
            });

            return mapper.Map<OrderReadDto>(order);
        }

        public async Task<bool> UpdateAsync(int id, OrderUpdateDto dto, CancellationToken ct = default)
        {
            await updateValidator.ValidateAndThrowAsync(dto);
            var order = await repository.GetByIdAsync(id);
            if (order is null) return false;
            mapper.Map(dto, order);
            await repository.UpdateAsync(order, ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var order = await repository.GetByIdAsync(id);
            if (order is null) return false;
            await repository.DeleteAsync(order, ct);
            return true;
        }
    }
}
