using FluentValidation.TestHelper;
using MyModularStore.Orders.Application.DTOs;
using MyModularStore.Orders.Application.Validators;
using MyModularStore.Orders.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Orders.Tests.Validators
{
    public class OrderCreateDtoValidatorTests
    {
        private readonly OrderCreateDtoValidators _validator = new();


        //Customer Id

        [Fact]
        public async Task CustomerId_Null_ShouldFailValidation()
        {
            // Arrange
            OrderCreateDto oder = CreateOrderDto(null);

            // Act
            TestValidationResult<OrderCreateDto> result = await _validator.TestValidateAsync(oder);

            // Assertion
            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task CustomerId_ZeroOrNegative_ShouldFailValidation(int id)
        {
            var result = await _validator.TestValidateAsync(CreateOrderDto(id));
            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
            result.ShouldNotHaveValidationErrorFor(x => x.CustomerId);
        }



        private static OrderCreateDto CreateOrderDto(int? customerId) => new()
        {
            CustomerId = customerId,
            OrderNumber = "ORD-TEST",
            TotalAmount = 150.00m,
            Status = OrderStatus.Pending,
        };
    }
}
