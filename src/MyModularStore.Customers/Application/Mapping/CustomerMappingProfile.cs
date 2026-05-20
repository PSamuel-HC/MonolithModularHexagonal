
using MyModularStore.Customers.Application.DTOs;
using MyModularStore.Customers.Domain;
using AutoMapper;

namespace MyModularStore.Customers.Application.Mapping
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();

            CreateMap<Customer, CustomerCreateDto>().ReverseMap();

            CreateMap<Customer, CustomerUpdateDto>().ReverseMap();

        }
    }
}
