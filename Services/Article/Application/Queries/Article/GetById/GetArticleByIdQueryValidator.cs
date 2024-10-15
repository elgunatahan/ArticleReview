using FluentValidation;

namespace Application.Queries.Article.GetById
{
    public class GetArticleByIdQueryValidator : AbstractValidator<GetArticleByIdQuery>
    {
        public GetArticleByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
