using AutoMapper;
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Logging;
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
        IPublishEndpoint publishEnpoint,
        ILogger<OrderService> logger
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
            if (order is null)
                logger.LogWarning("Order {OrderId} not found", id);
            return order is null ? null : mapper.Map<OrderReadDto>(order);
        }

        public async Task<OrderReadDto> CreateAsync(OrderCreateDto dto, CancellationToken ct = default)
        {
            await createValidator.ValidateAndThrowAsync(dto, ct);  //B1

            //var customerExists = await customerContract.ExistsAsync(dto.CustomerId!.Value); //B2
            //if (!customerExists)
            //{
            //    logger.LogWarning(
            //        "Order creation rejected — customer {CustomerId} not found", dto.CustomerId);
            //    throw new NotFoundException($"Customer with id {dto.CustomerId} not found.");
            //}

            var order = mapper.Map<Order>(dto);
            await repository.AddAsync(order);

            logger.LogInformation(
                "Order {OrderId} ({OrderNumber}) created for customer {CustomerId}",
                order.Id, order.OrderNumber, order.CustomerId);

            // Topic — fan-out to all subscribers
            await publishEnpoint.Publish(new OrderPlacedEvent
            {
                OrderId = order.Id,
                CustomerId = order.CustomerId,
                OrderNumber = order.OrderNumber,
                PlacedAt = DateTime.UtcNow
            });

            logger.LogInformation("OrderPlacedEvent published for order {OrderId}", order.Id);

            return mapper.Map<OrderReadDto>(order);
        }

        public async Task<bool> UpdateAsync(int id, OrderUpdateDto dto, CancellationToken ct = default)
        {
            await updateValidator.ValidateAndThrowAsync(dto);
            var order = await repository.GetByIdAsync(id);
            if (order is null)
            {
                logger.LogWarning("Update requested for non-existent order {OrderId}", id);
                return false;
            }
            mapper.Map(dto, order);
            await repository.UpdateAsync(order, ct);
            logger.LogInformation("Order {OrderId} updated", id);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var order = await repository.GetByIdAsync(id);
            if (order is null)
            {
                logger.LogWarning("Delete requested for non-existent order {OrderId}", id);
                return false;
            }
            await repository.DeleteAsync(order, ct);
            logger.LogInformation("Order {OrderId} deleted", id);
            return true;
        }

        public Task<IEnumerable<OrderWithCustomerReadDto>> GetAllWithCustomerAsync(
            CancellationToken ct = default)
            => repository.GetAllWithCustomerAsync(ct);
    }
}
