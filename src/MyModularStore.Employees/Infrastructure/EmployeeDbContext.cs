using Microsoft.EntityFrameworkCore;
using MyModularStore.Employees.Domain;

namespace MyModularStore.Employees.Infrastructure
{
    public class EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : DbContext(options)
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("employees");
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("employees");
                entity.Property(e => e.HourlyRate).HasColumnType("decimal(18,2)");
            });
        }
    }
}
