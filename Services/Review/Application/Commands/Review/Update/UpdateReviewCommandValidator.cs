using FluentValidation;

namespace Application.Commands.Review.Update
{
    public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
    {
        public UpdateReviewCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.ReviewContent).NotEmpty();
            RuleFor(x => x.Reviewer).NotEmpty();
            RuleFor(x => x.ArticleId).NotEmpty();
        }
    }
}
