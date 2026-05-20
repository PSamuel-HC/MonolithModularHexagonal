using AutoMapper;
using MyModularStore.Orders.Aplication.DTOs;
using MyModularStore.Orders.Domain.Entities;

namespace MyModularStore.Orders.Aplication.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Order, OrderReadDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

            CreateMap<OrderCreateDto, Order>();
        }
    }
}
