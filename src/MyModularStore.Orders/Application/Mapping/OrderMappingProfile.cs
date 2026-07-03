using AutoMapper;
using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Domain.Entities;

namespace MyModularStore.Orders.Application.Mapping
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderReadDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

            CreateMap<OrderCreateDto, Order>()
                .ForMember(d => d.OrderNumber,
                    opt => opt.MapFrom(_ => "ORD-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()))
                .ForMember(d => d.CreatedAt, opt => opt.Ignore());

            CreateMap<OrderUpdateDto, Order>()
                .ForMember(d => d.OrderNumber, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt,   opt => opt.Ignore());
        }
    }
}
