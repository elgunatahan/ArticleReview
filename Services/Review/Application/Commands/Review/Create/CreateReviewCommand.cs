using MediatR;

namespace Application.Commands.Review.Create
{
    public class CreateReviewCommand : IRequest<CreateReviewRepresentation>
    {
        public string ReviewContent { get; set; }
        public string Reviewer { get; set; }
        public Guid ArticleId { get; set; }
    }
}
