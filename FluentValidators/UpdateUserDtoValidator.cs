using EFCorePracticeAPI.Data;
using EFCorePracticeAPI.FluentValidators.Custom;
using EFCorePracticeAPI.ViewModals.User;
using FluentValidation;

namespace EFCorePracticeAPI.FluentValidators
{
    public class UpdateUserDtoValidator : AbstractValidator<V_UpdateUser>
    {
        public UpdateUserDtoValidator(AppDbContext context)
        {
            RuleFor(x => x.Password!)
                .MustNotContainWhitespace()
                .MinimumLength(6).WithMessage("Password have to >= 6 charactor")
                .When(x => !string.IsNullOrWhiteSpace(x.Password));

            RuleFor(x => x.Fullname)
                .Length(3, 50)
                .WithMessage("Fullname must be between 3 and 50 characters long.")
                .When(x => !string.IsNullOrWhiteSpace(x.Fullname));

            RuleFor(x => x.Email!)
                .MustUniqueEmail(context)
                .MustBeStrictEmail().When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.RoleIds)
                .Must(roleIds => roleIds!.Distinct().Count() == roleIds!.Count)
                .WithMessage("RoleIds cannot be duplicate.");
        }
    }
}
