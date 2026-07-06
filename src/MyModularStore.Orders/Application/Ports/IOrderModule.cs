using MyModularStore.Orders.Application.DTOs;

namespace MyModularStore.Orders.Application.Ports
{
    public interface IOrderModule
    {
        Task<IEnumerable<OrderReadDto>> GetAllAsync(CancellationToken ct = default);
        Task<OrderReadDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<OrderReadDto> CreateAsync(OrderCreateDto dto, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, OrderUpdateDto dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<OrderWithCustomerReadDto>> GetAllWithCustomerAsync(CancellationToken ct = default);
    }
}
