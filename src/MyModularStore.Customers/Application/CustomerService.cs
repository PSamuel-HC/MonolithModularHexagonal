using AutoMapper;
using FluentValidation;
using MyModularStore.Customers.Application.DTOs;
using MyModularStore.Customers.Application.Ports;
using MyModularStore.Customers.Application.Validators;
using MyModularStore.Customers.Domain;
using MyModularStore.Shared.Contracts;
using MyModularStore.Shared.Exceptions;

namespace MyModularStore.Customers.Application
{
    public  class CustomerService(
       ICustomerRepository repository,
       IMapper mapper,
       CustomerCreateDtoValidator createValidator,
       CustomerUpdateDtoValidator updateValidator) : ICustomerModule,  ICustomerContract
    {
        public async Task<IEnumerable<CustomerDto>> GetCustomersAsync()
        {
            IEnumerable<Customer> customer = await repository.GetAllAsync();
            return mapper.Map<IEnumerable<CustomerDto>>(customer);
        }

        public async Task<CustomerDto> CreateCustomerAsync(CustomerCreateDto dto)
        {
            await createValidator.ValidateAndThrowAsync(dto);
            Customer customer = mapper.Map<Customer>(dto);
            await repository.CreateAsync(customer);
            return mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> GetOneCustomerAsync(int id)
        {
            Customer? customer = await repository.GetOneAsync(id);
            return mapper.Map<CustomerDto>(customer);
        }

        public async Task<bool> UpdateCustomerAsync(int id, CustomerUpdateDto customerUpdateDto)
        {
            await updateValidator.ValidateAndThrowAsync(customerUpdateDto);

            var customer = await repository.GetOneAsync(id);

            if (customer == null)
            {
                return false;
            }

            mapper.Map(customerUpdateDto, customer);

            await repository.UpdateAsync(customer);

            return true;
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await repository.GetOneAsync(id);

            if (customer == null)
            {
                return false;
            }

            await repository.DeleteAsync(customer);

            return true;
        }


        public async Task<CustomerInfo?> GetInfoAsync(int id)
        {
            var customer = await repository.GetOneAsync(id);
            return customer is null ? null : new CustomerInfo() {
                Id = customer.Id, 
                FullName = customer.FullName,
                Email = customer.Email, IsPremium = customer.IsPremium,
                PointsBalance = customer.PointsBalance,
                LastPurchaseDate = customer.LastPurchaseDate
            };
        }

        public async Task<bool> ExistsAsync(int id)
        {
            Customer? customer = await repository.GetOneAsync(id);
            if(customer is null)
            {
                throw new NotFoundException($"Customer with Id {id} Not found");
            }
            return true;
        }
    }
}
