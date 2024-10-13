using FluentValidation;

namespace Application.Commands.Article.Create
{
    public class CreateArticleCommandValidator : AbstractValidator<CreateArticleCommand>
    {
        public CreateArticleCommandValidator()
        {
            RuleFor(x => x.ArticleContent).NotEmpty();
            RuleFor(x => x.Author).NotEmpty();
            RuleFor(x => x.StarCount).GreaterThan(0);
            RuleFor(x => x.Title).NotEmpty();
        }
    }
}
