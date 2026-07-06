using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Domain.Entities;

namespace MyModularStore.Orders.Application.Ports
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default);
        Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Order order, CancellationToken ct = default);
        Task UpdateAsync(Order order, CancellationToken ct = default);
        Task DeleteAsync(Order order, CancellationToken ct = default);
        Task<IEnumerable<OrderWithCustomerReadDto>> GetAllWithCustomerAsync(CancellationToken ct = default);
    }
}
