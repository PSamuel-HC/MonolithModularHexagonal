using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyModularStore.Customers.Application;
using MyModularStore.Customers.Application.Ports;
using MyModularStore.Customers.Application.Validators;
using MyModularStore.Customers.Infrastructure;
using MyModularStore.Shared.Contracts;

namespace MyModularStore.Customers
{
    public static class CustomerModuleExtensions
    {
        public static IServiceCollection AddCustomersModule(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<CustomerDbContext>(options => options.UseNpgsql(conn));

            services.AddScoped<ICustomerRepository, CustomerRepository> ();

            services.AddScoped<CustomerCreateDtoValidator>();
            services.AddScoped<CustomerUpdateDtoValidator>();

            services.AddAutoMapper(cfg => { }, typeof(CustomerModuleExtensions).Assembly);

            services.AddScoped<CustomerService>();
            services.AddScoped<ICustomerModule>(sp => sp.GetRequiredService<CustomerService>());
            //services.AddScoped<ICustomerContract>(sp => sp.GetRequiredService<CustomerService>());
            return services;
        }
    }
}
