using EFCorePracticeAPI.ViewModals;
using FluentValidation;

namespace EFCorePracticeAPI.FluentValidators
{
    public class GetAllDtolValidator : AbstractValidator<SearchDto>
    {
        public GetAllDtolValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("PageSize must be greater than 0.");

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.Search));
        }
    }
}
