using FluentValidation;

namespace Application.Commands.Review.Delete
{
    public class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
    {
        public DeleteReviewCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
