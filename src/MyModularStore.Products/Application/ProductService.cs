using AutoMapper;
using FluentValidation;
using MyModularStore.Products.Application.DTOs;
using MyModularStore.Products.Application.Ports;
using MyModularStore.Products.Application.Validators;
using MyModularStore.Products.Domain;
using MyModularStore.Shared.Contracts;

namespace MyModularStore.Products.Application
{
    public class ProductService(
    IProductRepository repository,
    IMapper mapper,
    CreateProductDtoValidator createValidator,
    UpdateProductDtoValidator updateValidator)
    : IProductModule, IProductContract
    {
        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await repository.GetAllAsync();
            return mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await repository.GetByIdAsync(id);
            return product is null ? null : mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            await createValidator.ValidateAndThrowAsync(dto);
            var product = mapper.Map<Product>(dto);
            await repository.AddAsync(product);
            return mapper.Map<ProductDto>(product);
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            await updateValidator.ValidateAndThrowAsync(dto);
            var product = await repository.GetByIdAsync(id);
            if (product is null) return false;
            mapper.Map(dto, product);
            await repository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await repository.GetByIdAsync(id);
            if (product is null) return false;
            await repository.DeleteAsync(product);
            return true;
        }

        public async Task<ProductInfo?> GetInfoAsync(int id)
        {
            var product = await repository.GetByIdAsync(id);
            return product is null ? null : new ProductInfo() { Id = product.Id, Name = product.Name, Price = product.Price };
        }

        public async Task<bool> ExistsAsync(int id)
            => await repository.GetByIdAsync(id) is not null;

    }
}
