using FluentValidation;
using MyModularStore.Customers.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyModularStore.Customers.Application.Validators
{
    public class CustomerBaseDtoValidator<T> : AbstractValidator<T> where T: CustomerBaseDto
    {
        private readonly List<string> invalidAddresses = ["blocked.com", "spam.com", "fake.com"];

        public CustomerBaseDtoValidator()
        {
            RuleFor(x => x.Email).Cascade(CascadeMode.Stop).
                                 NotEmpty().WithMessage("Email can not be empty").WithErrorCode("EM001").
                                 EmailAddress().WithMessage("Email doesn't have correct format").WithErrorCode("EM002").
                                 MaximumLength(100).WithMessage("Maximum Length is 100").WithErrorCode("EM003").

                                 MustAsync(async (email, token) => {
                                     await Task.Delay(1000);

                                     string[] emailSections = email.Split("@");
                                     if (emailSections.Length < 2) return false;
                                     return !invalidAddresses.Contains(emailSections[1]);

                                 }).WithMessage("This email domain is not allowed").WithErrorCode("EM004");

            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full name can not be empty").WithErrorCode("FN001").
                                    MinimumLength(2).WithMessage("Minimum length of fullname is 2").WithErrorCode("FN002").
                                    MaximumLength(150).WithMessage("Maximun length of fullname is 150").WithErrorCode("FN003").
                                    Must(y => !y.Contains("  ")).WithMessage("Full name can not have concecutive spaces").WithErrorCode("FN004").
                                    MinimumLength(5).When(x => x.IsPremium, ApplyConditionTo.CurrentValidator).WithMessage("When the customer is premium, minimum length of fullname is 5").WithErrorCode("FN005");
        }
    }
}
