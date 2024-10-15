using Application.Exceptions;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Application.Queries.Review.GetById
{
    public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, GetReviewByIdRepresentation>
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IDistributedCache _cache;

        public GetReviewByIdQueryHandler(IReviewRepository reviewRepository, IDistributedCache cache)
        {
            _reviewRepository = reviewRepository;
            _cache = cache;
        }

        public async Task<GetReviewByIdRepresentation> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
        {
            var cachedReview = await _cache.GetStringAsync(request.Id.ToString(), cancellationToken);

            if (!string.IsNullOrEmpty(cachedReview))
            {
                return JsonSerializer.Deserialize<GetReviewByIdRepresentation>(cachedReview);
            }

            Domain.Entities.Review review = await _reviewRepository.GetByIdAsync(request.Id, cancellationToken);

            if(review == null)
            {
                throw new ReviewNotFoundException(request.Id);
            }

            var representation = new GetReviewByIdRepresentation(review);

            await _cache.SetStringAsync(request.Id.ToString(), JsonSerializer.Serialize(representation), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            }, cancellationToken);

            return representation;
        }
    }
}
