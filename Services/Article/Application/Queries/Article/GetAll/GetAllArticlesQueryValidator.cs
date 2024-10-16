using Application.Queries.Article.GetAll;
using FluentValidation;

public class GetAllArticlesQueryValidator : AbstractValidator<GetAllArticlesQuery>
{
    public GetAllArticlesQueryValidator()
    {
        RuleFor(query => query.QueryOptions).NotNull().WithMessage("Query options are required.");
    }
}