using EFCorePracticeAPI.Data;
using EFCorePracticeAPI.FluentValidators.Custom;
using EFCorePracticeAPI.ViewModals.Role;
using FluentValidation;

namespace EFCorePracticeAPI.FluentValidators
{
    public class CreateAndUpdateRoleDtoValidator : AbstractValidator<V_Role>
    {
        public CreateAndUpdateRoleDtoValidator(AppDbContext context)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Role name is required.")
                .MustNotContainWhitespace()
                .MustUniqueRole(context)
                .Length(3, 50)
                .WithMessage("Role name must be between 3 and 50 characters long.");
        }
    }
}
