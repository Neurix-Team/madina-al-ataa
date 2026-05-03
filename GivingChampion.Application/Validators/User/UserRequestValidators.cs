using FluentValidation;
using GivingChampion.Application.DTO.User;
using GivingChampion.Application.Validators.Common;
using GivingChampion.Common.Enums;

namespace GivingChampion.Application.Validators.User
{
    public sealed class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Email).ValidEmail();
            RuleFor(x => x.Password).ValidPassword();
            RuleFor(x => x.FullName).ValidFullName();
            RuleFor(x => x.BirthDay).AdultBirthDate();
        }
    }

    public sealed class UpdateUserValidator : AbstractValidator<UpdateUser>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.FullName).ValidFullName();

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(250).WithMessage("Address must not exceed 250 characters.");

            RuleFor(x => x.BirthDay).AdultBirthDate();

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
                .Matches(@"^\+?[0-9\s\-()]{7,20}$").WithMessage("Phone number is invalid.");
        }
    }

    public sealed class DeleteUserValidator : AbstractValidator<DeleteUser>
    {
        public DeleteUserValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User id is required.");
        }
    }

    public sealed class RoleManagementRequestValidator : AbstractValidator<RoleManagementRequest>
    {
        private static readonly HashSet<string> AllowedRoles = Enum
            .GetNames<Roles>()
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        public RoleManagementRequestValidator()
        {
            RuleFor(x => x.Roles)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Roles are required.")
                .Must(roles => roles.Any()).WithMessage("At least one role is required.")
                .Must(roles =>
                {
                    var normalizedRoles = roles
                        .Where(role => !string.IsNullOrWhiteSpace(role))
                        .Select(role => role.Trim())
                        .ToList();

                    return normalizedRoles.Distinct(StringComparer.OrdinalIgnoreCase).Count() == normalizedRoles.Count;
                })
                .WithMessage("Roles must not contain duplicates.");

            RuleForEach(x => x.Roles)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Role name must not be empty.")
                .Must(role => AllowedRoles.Contains(role.Trim()))
                .WithMessage(role => $"Role '{role}' is not supported.");
        }
    }
}
