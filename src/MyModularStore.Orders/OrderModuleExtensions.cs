using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyModularStore.Orders.Application.Behaviors;
using MyModularStore.Orders.Application.Ports;
using MyModularStore.Orders.Application.Services;
using MyModularStore.Orders.Application.Validators;
using MyModularStore.Orders.Infrastructure;
using Npgsql;

namespace MyModularStore.Orders
{
    public static class OrderModuleExtensions
    {
        public static IServiceCollection AddOrdersModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("DefaultConnection")!;

            var dataSource = NpgsqlDataSource.Create(conn);
            services.AddSingleton(dataSource);

            // services.AddDbContext<OrderDBContext>(options => options.UseNpgsql(conn));

            services.AddScoped<IOrderRepository, NpgsqlOrderRepository>();

            services.AddScoped<OrderCreateDtoValidators>();
            services.AddScoped<OrderUpdateDtoValidators>();

            services.AddAutoMapper(cfg => { }, typeof(OrderModuleExtensions).Assembly);

            services.AddScoped<OrderService>();
            services.AddScoped<IOrderModule>(sp => sp.GetRequiredService<OrderService>());

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(OrderModuleExtensions).Assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            });

            return services;
        }
    }
}
