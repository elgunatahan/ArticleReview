using Application.Exceptions;
using Domain.Interfaces.Proxies;
using Domain.Repositories;
using MediatR;

namespace Application.Commands.Review.Update
{
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IArticleApiProxy _articleApiProxy;

        public UpdateReviewCommandHandler(IReviewRepository reviewRepository, IArticleApiProxy articleApiProxy)
        {
            _reviewRepository = reviewRepository;
            _articleApiProxy = articleApiProxy;
        }

        public async Task Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
        {
            var article = await _articleApiProxy.GetByIdAsync(request.ArticleId);

            if (article == null)
            {
                throw new ArticleNotExistException(request.ArticleId);
            }
            Domain.Entities.Review review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);

            if (review == null)
            {
                throw new ReviewNotFoundException(request.Id);
            }

            review.Update(request.ArticleId, request.Reviewer, request.ReviewContent);

            await _reviewRepository.UpdateAsync(review, cancellationToken);
        }
    }
}
