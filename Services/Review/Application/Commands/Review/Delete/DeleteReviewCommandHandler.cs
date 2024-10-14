using Application.Exceptions;
using Domain.Repositories;
using MediatR;

namespace Application.Commands.Review.Delete
{
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand>
    {
        private readonly IReviewRepository _reviewRepository;

        public DeleteReviewCommandHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.Review review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);

            if(review == null)
            {
                throw new ReviewNotFoundException(request.Id);
            }

            review.Delete();

            await _reviewRepository.UpdateAsync(review, cancellationToken);
        }
    }
}
