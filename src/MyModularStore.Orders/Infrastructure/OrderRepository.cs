using Microsoft.EntityFrameworkCore;
using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Application.Ports;
using MyModularStore.Orders.Domain.Entities;

namespace MyModularStore.Orders.Infrastructure
{
    public class OrderRepository(OrderDBContext context) : IOrderRepository
    {
        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken ct = default) =>
            await context.Orders.ToListAsync(ct);

        public async Task<Order?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await context.Orders.FindAsync(id, ct);

        public async Task AddAsync(Order order, CancellationToken ct = default)
        {
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Order order, CancellationToken ct = default)
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Order order, CancellationToken ct = default)
        {
            context.Orders.Remove(order);
            await context.SaveChangesAsync(ct);
        }

        public Task<IEnumerable<OrderWithCustomerReadDto>> GetAllWithCustomerAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
