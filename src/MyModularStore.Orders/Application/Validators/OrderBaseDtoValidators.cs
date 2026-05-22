using FluentValidation;
using MyModularStore.Orders.Application.DTOs;

namespace MyModularStore.Orders.Application.Validators
{
    public class OrderBaseDtoValidators<T> : AbstractValidator<T> where T : OrderBaseDto
    {
        public OrderBaseDtoValidators()
        {
            RuleFor(x => x.OrderNumber)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Order number cannot be empty and has a maximum length of 50 characters.");

            RuleFor(x => x.CustomerId)
                .GreaterThan(0)
                .WithMessage("A valid CustomerId is required.");

            RuleFor(x => x.CustomerName)
                .NotEmpty()
                .MaximumLength(200)
                .Must(name => !name!.Contains("  "))
                .WithMessage("Customer name cannot be empty, exceed 200 characters, or contain two continuous spaces.");

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0)
                .WithMessage("Total amount must be greater than 0.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Status must be Pending, Shipped, or Delivered.");

            RuleFor(x => x.ShippingAddress)
                .MaximumLength(500)
                .WithMessage("Shipping address must be less than 500 characters.");

            RuleFor(x => x.ShippingAddress)
                .NotEmpty()
                .When(x => x.Status == Domain.Enums.OrderStatus.Shipped || x.Status == Domain.Enums.OrderStatus.Delivered)
                .WithMessage("Shipping address is required when status is Shipped or Delivered.");
        }
    }
}
