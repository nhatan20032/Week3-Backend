using EFCorePracticeAPI.Data;
using EFCorePracticeAPI.FluentValidators.Custom;
using EFCorePracticeAPI.ViewModals.Role;
using FluentValidation;

namespace EFCorePracticeAPI.FluentValidators
{
    public class UpdateRoleDtoValidator : AbstractValidator<V_UpdateRole>
    {
        public UpdateRoleDtoValidator(AppDbContext context)
        {
            RuleFor(x => x.IsDefault)
                .NotNull()
                .WithMessage("IsDefault is required.")
                .MustUniqueIsDefault(context);
        }
    }
}
