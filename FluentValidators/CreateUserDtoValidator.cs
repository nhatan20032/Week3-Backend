using EFCorePracticeAPI.FluentValidators.Custom;
using EFCorePracticeAPI.ViewModals.User;
using FluentValidation;
using System.Text.RegularExpressions;

namespace EFCorePracticeAPI.FluentValidators
{
    public sealed class CreateUserDtoValidator : AbstractValidator<V_CreateUser>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotNull()
                .WithMessage("Username is required.")
                .MustNotContainWhitespace()
                .Length(3, 50)
                .WithMessage("Username must be between 3 and 50 characters long.");

            RuleFor(x => x.Password)
                .MustNotContainWhitespace()
                .MinimumLength(6).WithMessage("Password have to >= 6 charactor");

            RuleFor(x => x.Fullname)
                .NotEmpty()
                .WithMessage("Fullname is required.")
                .Length(3, 50)
                .WithMessage("Fullname must be between 3 and 50 characters long.");

            RuleFor(x => x.Email!)
                .NotEmpty()
                .WithMessage("Email is required.")
                .MustBeStrictEmail();

            RuleFor(x => x.RoleIds)
                .NotEmpty().WithMessage("RoleIds không được rỗng.")
                .Must(roleIds => roleIds!.Distinct().Count() == roleIds!.Count)
                .WithMessage("RoleIds cannot be duplicate.");

            RuleForEach(x => x.RoleIds)
                        .Must(roleId => roleId > 0)
                        .When(x => x.RoleIds != null && x.RoleIds.Count != 0)
                        .WithMessage("All RoleIds must be positive integers like 1, 2, 3...");

        }
    }
}
