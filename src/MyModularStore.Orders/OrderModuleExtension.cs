
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyModularStore.Orders.Aplication.Ports;
using MyModularStore.Orders.Aplication.Services;
using MyModularStore.Orders.Aplication.Validators;
using MyModularStore.Orders.Interfaces;

namespace MyModularStore.Orders
{
    public static class OrderModuleExtension
    {
        public static IServiceCollection AddOrdersModule(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var conn = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<OrderDBContext>(options => options.UseNpgsql(conn));

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<OrderCreateDtoValidators>();
            services.AddScoped<OrderUpdateDtoValidators>();

            services.AddAutoMapper(cfg => { }, typeof(OrderModuleExtension).Assembly);

            services.AddScoped<OrderService>();
            services.AddScoped<IOrderModule>(sp => sp.GetRequiredService<OrderService>());

            return services;
        }
    }
}
