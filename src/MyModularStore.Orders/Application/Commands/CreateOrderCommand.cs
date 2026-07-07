using MediatR;
using MyModularStore.Orders.Application.DTOs;

namespace MyModularStore.Orders.Application.Commands
{
    public record CreateOrderCommand(OrderCreateDto dto) : IRequest<OrderReadDto>
    {
    }
}
