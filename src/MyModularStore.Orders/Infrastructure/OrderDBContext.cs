using MassTransit;
using Microsoft.EntityFrameworkCore;
using MyModularStore.Orders.Domain.Entities;

namespace MyModularStore.Orders.Infrastructure
{
    public class OrderDBContext(DbContextOptions<OrderDBContext> options) : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("orders");

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }
    }
}
