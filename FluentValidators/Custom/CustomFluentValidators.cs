using EFCorePracticeAPI.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
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

        public static IRuleBuilderOptions<T, string> MustUniqueUsername<T>(
        this IRuleBuilder<T, string> ruleBuilder, AppDbContext context)
        {
            return ruleBuilder.Must(username =>
            {
                return !context.Users.Any(u => u.Username == username);
            }).WithMessage("Username already exist!");
        }

        public static IRuleBuilderOptions<T, string> MustUniqueEmail<T>(
        this IRuleBuilder<T, string> ruleBuilder, AppDbContext context)
        {
            return ruleBuilder.Must(email =>
            {
                return !context.Users.Any(u => u.Email == email);
            }).WithMessage("Email already exist!");
        }
    }
}
