using Microsoft.EntityFrameworkCore;
using MyModularStore.Customers.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Customers.Infrastructure
{
    public class CustomerDbContext(DbContextOptions<CustomerDbContext> options) : DbContext(options)
    {
        public DbSet<Customer> Customers {  get; set; }
    }
}
