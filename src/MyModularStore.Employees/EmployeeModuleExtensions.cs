using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyModularStore.Employees.Application;
using MyModularStore.Employees.Application.Ports;
using MyModularStore.Employees.Application.Validators;
using MyModularStore.Employees.Infrastructure;
using MyModularStore.Shared.Contracts;

namespace MyModularStore.Employees
{
    public static class EmployeeModuleExtensions
    {
        public static IServiceCollection AddEmployeesModule(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<EmployeeDbContext>(options => options.UseNpgsql(conn));

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            services.AddScoped<CreateEmployeeDtoValidator>();
            services.AddScoped<UpdateEmployeeDtoValidator>();

            services.AddAutoMapper(cfg => { }, typeof(EmployeeModuleExtensions).Assembly);

            services.AddScoped<EmployeeService>();
            services.AddScoped<IEmployeeModule>(sp => sp.GetRequiredService<EmployeeService>());
            //services.AddScoped<IEmployeeContract>(sp => sp.GetRequiredService<EmployeeService>());

            return services;
        }
    }
}
