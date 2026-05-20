using Microsoft.EntityFrameworkCore;
using MyModularStore.Employees.Application.Ports;
using MyModularStore.Employees.Domain;

namespace MyModularStore.Employees.Infrastructure
{
    public class EmployeeRepository(EmployeeDbContext context) : IEmployeeRepository
    {
        public async Task<IEnumerable<Employee>> GetAllAsync()
            => await context.Employees.ToListAsync();

        public async Task<Employee?> GetByIdAsync(int id)
            => await context.Employees.FindAsync(id);

        public async Task AddAsync(Employee employee)
        {
            await context.Employees.AddAsync(employee);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            context.Employees.Update(employee);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Employee employee)
        {
            context.Employees.Remove(employee);
            await context.SaveChangesAsync();
        }
    }
}
