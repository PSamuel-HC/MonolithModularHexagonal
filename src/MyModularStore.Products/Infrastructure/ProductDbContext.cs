using Microsoft.EntityFrameworkCore;
using MyModularStore.Products.Domain;

namespace MyModularStore.Products.Infrastructure
{
    public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("products");
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            });
        }
    }
}
