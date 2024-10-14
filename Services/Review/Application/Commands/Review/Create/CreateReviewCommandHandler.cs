using Application.Exceptions;
using Domain.Interfaces.Proxies;
using Domain.Repositories;
using MediatR;

namespace Application.Commands.Review.Create
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, CreateReviewRepresentation>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IArticleApiProxy _articleApiProxy;

        public CreateReviewCommandHandler(IReviewRepository reviewRepository, IArticleApiProxy articleApiProxy)
        {
            _reviewRepository = reviewRepository;
            _articleApiProxy = articleApiProxy;
        }

        public async Task<CreateReviewRepresentation> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        {
            var article = await _articleApiProxy.GetByIdAsync(request.ArticleId);

            if(article == null)
            {
                throw new ArticleNotExistException(request.ArticleId);
            }

            var review = new Domain.Entities.Review(request.ArticleId, request.Reviewer, request.ReviewContent);

            await _reviewRepository.CreateAsync(review, cancellationToken);

            return new CreateReviewRepresentation()
            {
                Id = review.IdentityObject.Id
            };
        }
    }
}
