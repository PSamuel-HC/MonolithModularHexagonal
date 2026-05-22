using MyModularStore.Orders.Application.DTOs;

namespace MyModularStore.Orders.Application.Ports
{
    public interface IOrderModule
    {
        Task<IEnumerable<OrderReadDto>> GetAllAsync();
        Task<OrderReadDto?> GetByIdAsync(int id);
        Task<OrderReadDto> CreateAsync(OrderCreateDto dto);
        Task<bool> UpdateAsync(int id, OrderUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
