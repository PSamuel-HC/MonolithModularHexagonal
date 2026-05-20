using MyModularStore.Customers.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Customers.Application.Ports
{
    public interface ICustomerModule
    {
        public Task<IEnumerable<CustomerDto>> GetCustomersAsync();
        public Task<CustomerDto> CreateCustomerAsync(CustomerCreateDto dto);
        public Task<CustomerDto> GetOneCustomerAsync(int id);
        Task<bool> UpdateCustomerAsync(int id, CustomerUpdateDto customerUpdateDto);
        Task<bool> DeleteCustomerAsync(int id);
    }
}
