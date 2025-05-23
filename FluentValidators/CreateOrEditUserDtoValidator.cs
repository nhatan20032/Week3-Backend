using EFCorePracticeAPI.ViewModals.User;
using FluentValidation;

namespace EFCorePracticeAPI.FluentValidators
{
    public sealed class CreateOrEditUserDtoValidator : AbstractValidator<V_User>
    {
        public CreateOrEditUserDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required.")
                .Length(3, 50)
                .WithMessage("Username must be between 3 and 50 characters long.");

            RuleFor(x => x.Password)
                .MinimumLength(6).WithMessage("Password have to >= 6 charactor");

            RuleFor(x => x.Fullname)
                .NotEmpty()
                .WithMessage("Fullname is required.")
                .Length(3, 50)
                .WithMessage("Fullname must be between 3 and 50 characters long.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Invalid email format.");
        }
    }
}
