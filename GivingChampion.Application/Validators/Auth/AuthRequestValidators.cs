using FluentValidation;
using GivingChampion.Application.DTO.Auth;
using GivingChampion.Application.Validators.Common;

namespace GivingChampion.Application.Validators.Auth
{
    public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email).ValidEmail();
            RuleFor(x => x.Password).ValidPassword();
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.")
                .Equal(x => x.Password).WithMessage("Confirm password must match password.");
            RuleFor(x => x.Fullname).ValidFullName();
            RuleFor(x => x.BirthDate).AdultBirthDate();
        }
    }

    public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).ValidEmail();
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }

   
    public sealed class CompleteSocialRegistrationRequestValidator : AbstractValidator<CompleteSocialRegistrationRequest>
    {
        public CompleteSocialRegistrationRequestValidator()
        {
            RuleFor(x => x.Userid)
                .NotEmpty().WithMessage("User id is required.")
                .Must(id => Guid.TryParse(id, out var parsedId) && parsedId != Guid.Empty)
                .WithMessage("User id must be a valid GUID.");

            RuleFor(x => x.Newpassword).ValidPassword();
            RuleFor(x => x.BirthDate).AdultBirthDate();
        }
    }

    public sealed class ExchangeCodeRequestValidator : AbstractValidator<ExchangeCodeRequest>
    {
        public ExchangeCodeRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(512).WithMessage("Code must not exceed 512 characters.");
        }
    }
}
