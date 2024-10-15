using FluentValidation;

namespace Application.Queries.Review.GetById
{
    public class GetReviewByIdQueryValidator : AbstractValidator<GetReviewByIdQuery>
    {
        public GetReviewByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
