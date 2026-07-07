using AutoMapper;
using MediatR;
using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Application.Ports;
using MyModularStore.Orders.Domain.Entities;

namespace MyModularStore.Orders.Application.Queries
{
    public class GetAllOrdersQueryHandler(
        IOrderRepository repository,
        IMapper mapper) : IRequestHandler<GetAllOrdersQuery, IEnumerable<OrderReadDto>>
    {
        public async Task<IEnumerable<OrderReadDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Order> orders = await repository.GetAllAsync(cancellationToken);
            return mapper.Map<IEnumerable<OrderReadDto>>(orders);
        }
    }
}
