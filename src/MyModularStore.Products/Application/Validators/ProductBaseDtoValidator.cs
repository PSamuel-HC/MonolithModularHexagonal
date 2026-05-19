using FluentValidation;
using MyModularStore.Products.Application.DTOs;

namespace MyModularStore.Products.Application.Validators
{
    public class ProductBaseDtoValidator<T> : AbstractValidator<T> where T : ProductBaseDto
    {
        public ProductBaseDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.Manufacturer).NotEmpty();
            RuleFor(x => x.WarrantyMonths).GreaterThanOrEqualTo(0);
        }
    }
}
