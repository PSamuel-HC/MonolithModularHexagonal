using FluentValidation;
using MyModularStore.Orders.Application.Commands;

namespace MyModularStore.Orders.Application.Validators
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator(OrderCreateDtoValidators dtoValidator)
        {
            RuleFor(cmd => cmd.dto).SetValidator(dtoValidator);
        }
    }
}
