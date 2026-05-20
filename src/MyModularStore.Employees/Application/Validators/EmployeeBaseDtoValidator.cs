using System.Globalization;
using FluentValidation;
using MyModularStore.Employees.Application.DTOs;

namespace MyModularStore.Employees.Application.Validators
{
    public class EmployeeBaseDtoValidator<T> : AbstractValidator<T> where T : EmployeeBaseDto
    {
        private static readonly string[] AllowedRoles = { "Junior", "Senior", "Manager" };

        public EmployeeBaseDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstName must not be empty.")
                .MaximumLength(100).WithMessage("FirstName cannot exceed 100 characters.")
                .Must(NoConsecutiveSpaces).WithMessage("FirstName must not contain consecutive spaces.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName must not be empty.")
                .MaximumLength(100).WithMessage("LastName cannot exceed 100 characters.")
                .Must(NoConsecutiveSpaces).WithMessage("LastName must not contain consecutive spaces.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role must not be empty.")
                .Must(role => AllowedRoles.Contains(role, StringComparer.Ordinal))
                .WithMessage("Role must be one of: Junior, Senior, Manager (case-sensitive).");

            RuleFor(x => x.HourlyRate)
                .GreaterThan(0).WithMessage("HourlyRate must be greater than 0.");

            RuleFor(x => x.HourlyRate)
                .GreaterThanOrEqualTo(25m).When(x => x.Role == "Manager")
                .WithMessage("HourlyRate must be at least $25 for Manager role.");

            RuleFor(x => x.HourlyRate)
                .GreaterThanOrEqualTo(15m).When(x => x.Role == "Senior")
                .WithMessage("HourlyRate must be at least $15 for Senior role.");

            RuleFor(x => x.HireDate)
                .NotEmpty().WithMessage("HireDate must not be empty.");

            RuleFor(x => x.HireDate)
                .Must(BeTodayOrPast)
                .When(x => !string.IsNullOrWhiteSpace(x.HireDate))
                .WithMessage("HireDate must be today or a date in the past.");
        }

        private static bool NoConsecutiveSpaces(string? value)
            => string.IsNullOrEmpty(value) || !value.Contains("  ");

        private static bool BeTodayOrPast(string hireDate)
        {
            if (DateTime.TryParse(hireDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date.Date <= DateTime.Today;
            return false;
        }
    }
}
