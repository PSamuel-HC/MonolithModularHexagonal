using FluentValidation;
using MyModularStore.Products.Application.DTOs;

namespace MyModularStore.Products.Application.Validators
{
    public class CreateProductDtoValidator : ProductBaseDtoValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.SKU).NotEmpty();
        }
    }
}
