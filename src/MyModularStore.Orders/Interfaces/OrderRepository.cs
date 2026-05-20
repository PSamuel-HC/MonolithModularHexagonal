using Microsoft.EntityFrameworkCore;
using MyModularStore.Orders.Aplication.Ports;
using MyModularStore.Orders.Domain.Entities;

namespace MyModularStore.Orders.Interfaces
{
    public class OrderRepository(OrderDBContext context) : IOrderRepository
    {
        public async Task<IEnumerable<Order>> GetAllAsync() => await context.Orders.ToListAsync();
        public async Task<Order?> GetByIdAsync(int id) => await context.Orders.FindAsync(id);
        public async Task AddAsync(Order order)
        {
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Order order)
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Order order)
        {
            context.Orders.Remove(order);
            await context.SaveChangesAsync();
        }
    }
}
