using BestStoriesApi.HackerNews.Queries;
using FluentValidation;

namespace BestStoriesApi.HackerNews.Validators;

public sealed class GetBestStoriesValidatorCollection : AbstractValidator<GetBestStoriesQuery>
{
    public GetBestStoriesValidatorCollection()
    {
        RuleFor(x => x.N)
            .GreaterThan(0);
    }
}