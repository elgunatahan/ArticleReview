using FluentValidation;

namespace Application.Commands.Article.Update
{
    public class UpdateArticleCommandValidator : AbstractValidator<UpdateArticleCommand>
    {
        public UpdateArticleCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.ArticleContent).NotEmpty();
            RuleFor(x => x.Author).NotEmpty();
            RuleFor(x => x.StarCount).GreaterThan(0);
            RuleFor(x => x.Title).NotEmpty();
        }
    }
}
