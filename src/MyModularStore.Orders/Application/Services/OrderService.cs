using AutoMapper;
using FluentValidation;
using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Application.Ports;
using MyModularStore.Orders.Application.Validators;
using MyModularStore.Orders.Domain.Entities;
using MyModularStore.Shared.Contracts;
using MyModularStore.Shared.Exceptions;

namespace MyModularStore.Orders.Application.Services
{
    public class OrderService(
        IOrderRepository repository,
        IMapper mapper,
        OrderCreateDtoValidators createValidator,
        OrderUpdateDtoValidators updateValidator,
        ICustomerContract customerContract
    ) : IOrderModule
    {
        public async Task<IEnumerable<OrderReadDto>> GetAllAsync()
        {
            var orders = await repository.GetAllAsync();
            return mapper.Map<IEnumerable<OrderReadDto>>(orders);
        }

        public async Task<OrderReadDto?> GetByIdAsync(int id)
        {
            var order = await repository.GetByIdAsync(id);
            return order is null ? null : mapper.Map<OrderReadDto>(order);
        }

        public async Task<OrderReadDto> CreateAsync(OrderCreateDto dto)
        {
            await createValidator.ValidateAndThrowAsync(dto);

            var customerExists = await customerContract.ExistsAsync(dto.CustomerId!.Value);
            if (!customerExists)
                throw new NotFoundException($"Customer with id {dto.CustomerId} not found.");

            var order = mapper.Map<Order>(dto);
            await repository.AddAsync(order);
            return mapper.Map<OrderReadDto>(order);
        }

        public async Task<bool> UpdateAsync(int id, OrderUpdateDto dto)
        {
            await updateValidator.ValidateAndThrowAsync(dto);
            var order = await repository.GetByIdAsync(id);
            if (order is null) return false;
            mapper.Map(dto, order);
            await repository.UpdateAsync(order);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await repository.GetByIdAsync(id);
            if (order is null) return false;
            await repository.DeleteAsync(order);
            return true;
        }
    }
}
