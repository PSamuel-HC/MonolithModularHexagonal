using MediatR;
using MyModularStore.Customers.Application.DTOs;

namespace MyModularStore.Customers.Features.GetCustomerById
{
    public record GetCustomerByIdQuery(int id) : IRequest<CustomerDto?>;
}
