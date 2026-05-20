using MyModularStore.Customers.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Customers.Application.Ports
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetOneAsync(int id);
        Task CreateAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Customer customer);

    }
}
