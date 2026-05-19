using AutoMapper;
using MyModularStore.Products.Application.DTOs;
using MyModularStore.Products.Domain;

namespace MyModularStore.Products.Application.Mapping
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.DisplayName, opt => opt.MapFrom(s => $"{s.Name} - {s.Manufacturer}"));

            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
        }
    }
}
