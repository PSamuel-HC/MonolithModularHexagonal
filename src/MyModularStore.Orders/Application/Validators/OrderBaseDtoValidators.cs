using FluentValidation;
using MyModularStore.Orders.Application.DTOs;

namespace MyModularStore.Orders.Application.Validators
{
    public class OrderBaseDtoValidators<T> : AbstractValidator<T> where T : OrderBaseDto
    {
        public OrderBaseDtoValidators()
        {
            RuleFor(x => x.CustomerId)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("A valid CustomerId is required.");

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0)
                .WithMessage("Total amount must be greater than 0.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Status must be Pending, Shipped, or Delivered.");
        }
    }
}
