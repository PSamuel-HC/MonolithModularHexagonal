using Microsoft.EntityFrameworkCore;
using MyModularStore.Products.Application.Ports;
using MyModularStore.Products.Domain;

namespace MyModularStore.Products.Infrastructure
{
    public class ProductRepository(ProductDbContext context) : IProductRepository
    {
        public async Task<IEnumerable<Product>> GetAllAsync()
            => await context.Products.ToListAsync();

        public async Task<Product?> GetByIdAsync(int id)
            => await context.Products.FindAsync(id);

        public async Task AddAsync(Product product)
        {
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            context.Products.Update(product);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
    }
}
