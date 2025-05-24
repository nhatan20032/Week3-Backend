using FluentValidation;
using System.Text.RegularExpressions;

namespace EFCorePracticeAPI.FluentValidators.Custom
{
    public static class CustomFluentValidators
    {        
        public static IRuleBuilderOptions<T, string> MustNotContainWhitespace<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Must(x => !Regex.IsMatch(x ?? "", @"\s"))
                .WithMessage("The field must not contain any whitespace.");
        }

        public static IRuleBuilderOptions<T, string> MustBeStrictEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Email is required.")
                .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("Email must be a valid format with a domain.");
        }
    }
}
