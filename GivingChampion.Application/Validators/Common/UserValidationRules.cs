using FluentValidation;

namespace GivingChampion.Application.Validators.Common
{
    internal static class UserValidationRules
    {
        public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");
        }

        public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .MaximumLength(128).WithMessage("Password must not exceed 128 characters.");
        }

        public static IRuleBuilderOptions<T, string> ValidFullName<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(150).WithMessage("Full name must not exceed 150 characters.");
        }

        public static IRuleBuilderOptions<T, DateTime> AdultBirthDate<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Birth date is required.")
                .LessThanOrEqualTo(DateTime.Today.AddYears(-18))
                .WithMessage("Your age must be 18 years or older.");
        }
    }
}
