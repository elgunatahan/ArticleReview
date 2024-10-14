using MediatR;

namespace Application.Commands.Review.Update
{
    public class UpdateReviewCommand : IRequest
    {
        public Guid Id { get; set; }
        public string ReviewContent { get; set; }
        public string Reviewer { get; set; }
        public Guid ArticleId { get; set; }
    }
}
