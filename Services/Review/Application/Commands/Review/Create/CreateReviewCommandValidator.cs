using FluentValidation;

namespace Application.Commands.Review.Create
{
    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(x => x.ReviewContent).NotEmpty();
            RuleFor(x => x.Reviewer).NotEmpty();
            RuleFor(x => x.ArticleId).NotEmpty();
        }
    }
}
