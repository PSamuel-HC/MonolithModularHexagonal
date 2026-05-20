using AutoMapper;
using MyModularStore.Orders.Aplication.DTOs;
using MyModularStore.Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
