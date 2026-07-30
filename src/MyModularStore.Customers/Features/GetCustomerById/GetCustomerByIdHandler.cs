using AutoMapper;
using MediatR;
using MyModularStore.Customers.Application.DTOs;
using MyModularStore.Customers.Application.Ports;
using MyModularStore.Customers.Domain;

namespace MyModularStore.Customers.Features.GetCustomerById
{
    public class GetCustomerByIdHandler(
        ICustomerRepository repository,
        IMapper mapper) : IRequestHandler<GetCustomerByIdQuery, CustomerDto?>
    {
        public async Task<CustomerDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            Customer? customer = await repository.GetOneAsync(request.id);
            return mapper.Map<CustomerDto>(customer);
        }
    }
}
