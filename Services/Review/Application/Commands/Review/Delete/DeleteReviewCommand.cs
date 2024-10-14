using MediatR;

namespace Application.Commands.Review.Delete
{
    public class DeleteReviewCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
