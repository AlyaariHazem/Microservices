using BuildingBlocks.Querying.Models;
using FluentValidation;

namespace BuildingBlocks.Querying.Validation
{
    /// <summary>
    /// Validator for QueryParameters
    /// </summary>
    public class QueryParametersValidator : AbstractValidator<QueryParameters>
    {
        public QueryParametersValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("Page size must be between 1 and 100.");

            RuleFor(x => x.Search)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.Search))
                .WithMessage("Search term cannot exceed 200 characters.");
        }
    }
}
