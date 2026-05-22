using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyModularStore.Orders.Application.Ports;
using MyModularStore.Orders.Application.Services;
using MyModularStore.Orders.Application.Validators;
using MyModularStore.Orders.Infrastructure;

namespace MyModularStore.Orders
{
    public static class OrderModuleExtensions
    {
        public static IServiceCollection AddOrdersModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<OrderDBContext>(options => options.UseNpgsql(conn));

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<OrderCreateDtoValidators>();
            services.AddScoped<OrderUpdateDtoValidators>();

            services.AddAutoMapper(cfg => { }, typeof(OrderModuleExtensions).Assembly);

            services.AddScoped<OrderService>();
            services.AddScoped<IOrderModule>(sp => sp.GetRequiredService<OrderService>());

            return services;
        }
    }
}
