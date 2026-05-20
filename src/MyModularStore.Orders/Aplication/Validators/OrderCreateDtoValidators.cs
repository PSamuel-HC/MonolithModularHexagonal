using FluentValidation;
using MyModularStore.Orders.Aplication.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Orders.Aplication.Validators
{
    public class OrderCreateDtoValidators : AbstractValidator<OrderCreateDto>
    {
        public OrderCreateDtoValidators()
        {
            RuleFor(x => x.OrderNumber)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Order number cannot be empty and has a maximun lenght of 50 characters");

            RuleFor(x => x.CustomerName)
                .NotEmpty()
                .MaximumLength(200)
                .Must(name => !name.Contains("  "))
                .WithMessage("Customer name cannot be empty, exceed 200 characters or contain two continous spaces");


            RuleFor(x => x.TotalAmount)
                .GreaterThan(0)
                .WithMessage("Total amount must be greater than 0.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Status has to be pending(1), shipped(2) or delivered(3).");

            RuleFor(x => x.ShippingAddress)
                .MaximumLength(500)
                .WithMessage("Shipping address has to less than 500 characters.");

            RuleFor(x => x.ShippingAddress)
                .NotEmpty()
                .When(x => x.Status == Domain.Enums.OrderStatus.Shipped || x.Status == Domain.Enums.OrderStatus.Delivered)
                .WithMessage("If order status is shipped or delivered shipping address can not be empty or exceed 500.");
        }
    }
}
