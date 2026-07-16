// using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using MyModularStore.Products.Application;
using MyModularStore.Products.Application.Ports;
using MyModularStore.Products.Application.Validators;
using MyModularStore.Products.Infrastructure;
using MyModularStore.Shared.Contracts;

namespace MyModularStore.Products
{
    public static class ProductModuleExtensions
    {
        public static IServiceCollection AddProductsModule(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("DefaultConnection");

            // services.AddDbContext<ProductDbContext>(options => options.UseNpgsql(conn));

            var dataSource = NpgsqlDataSource.Create(conn!);
            services.AddSingleton(dataSource);

            // services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductRepository, NpgsqlProductRepository>();

            services.AddScoped<CreateProductDtoValidator>();
            services.AddScoped<UpdateProductDtoValidator>();

            services.AddAutoMapper(cfg => { }, typeof(ProductModuleExtensions).Assembly);

            services.AddScoped<ProductService>();
            services.AddScoped<IProductModule>(sp => sp.GetRequiredService<ProductService>());
            //services.AddScoped<IProductContract>(sp => sp.GetRequiredService<ProductService>());

            var cosmosConnStr = configuration.GetConnectionString("CosmosDb");
            if (!string.IsNullOrEmpty(cosmosConnStr))
            {
                services.AddSingleton(new CosmosClient(cosmosConnStr));
                services.AddScoped<CosmosProductReviewRepository>();
            }

            return services;
        }
    }
}
