using MediatR;
using MyModularStore.Orders.Application.DTOs;

namespace MyModularStore.Orders.Application.Queries
{
    public record GetAllOrdersQuery : IRequest<IEnumerable<OrderReadDto>>;
}
