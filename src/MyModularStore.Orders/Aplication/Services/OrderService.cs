using AutoMapper;
using FluentValidation;
using MyModularStore.Orders.Aplication.DTOs;
using MyModularStore.Orders.Aplication.Ports;
using MyModularStore.Orders.Aplication.Validators;
using MyModularStore.Orders.Domain.Entities;

namespace MyModularStore.Orders.Aplication.Services
{
    public class OrderService(
        IOrderRepository repository,
        IMapper mapper,
        OrderCreateDtoValidators createValidator,
        OrderUpdateDtoValidators updateValidator
    ) : IOrderModule // , IOrderContract
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
