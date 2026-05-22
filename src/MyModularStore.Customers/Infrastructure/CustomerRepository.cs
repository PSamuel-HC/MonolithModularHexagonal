using Microsoft.EntityFrameworkCore;
using MyModularStore.Customers.Application.Ports;
using MyModularStore.Customers.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Customers.Infrastructure
{
    internal class CustomerRepository(CustomerDbContext context ) : ICustomerRepository
    {
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await context.Customers.ToListAsync();
        }

        public async Task<Customer?> GetOneAsync(int id)
        {
            var customer = await context.Customers.FindAsync(id);
            return customer;
        }

        public async Task CreateAsync(Customer customer)
        {
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

        }

        public async Task UpdateAsync(Customer customer)
        {
            context.Customers.Update(customer);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Customer customer)
        {
            context.Customers.Remove(customer);
            await context.SaveChangesAsync();
        }


            
    }
}
