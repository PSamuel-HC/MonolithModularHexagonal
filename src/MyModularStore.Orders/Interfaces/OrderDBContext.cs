using Microsoft.EntityFrameworkCore;
using MyModularStore.Orders.Domain.Entities;

namespace MyModularStore.Orders.Interfaces
{
    public class OrderDBContext(DbContextOptions<OrderDBContext> options) : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }
    }
}
