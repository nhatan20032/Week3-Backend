using EFCorePracticeAPI.Dtos;
using EFCorePracticeAPI.FluentValidators.Custom;
using FluentValidation;

namespace EFCorePracticeAPI.FluentValidators
{
    public class LoginDtoValidator : AbstractValidator<LoginRequest>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username is required.")
                .MustNotContainWhitespace()
                .Length(3, 50)
                .WithMessage("Username must be between 3 and 50 characters long.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MustNotContainWhitespace()
                .MinimumLength(6).WithMessage("Password have to >= 6 charactor");
        }
    }
}
