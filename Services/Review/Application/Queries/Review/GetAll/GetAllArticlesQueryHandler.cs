using Domain.Dtos;
using Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;

namespace Application.Queries.Review.GetAll
{
    public class GetAllReviewsQueryHandler : IRequestHandler<GetAllReviewsQuery, IEnumerable<ReviewDto>>
    {
        private readonly IDistributedCache _cache;
        private readonly IReviewRepository _reviewRepository;

        public GetAllReviewsQueryHandler(IDistributedCache cache, IReviewRepository reviewRepository)
        {
            _cache = cache;
            _reviewRepository = reviewRepository;
        }

        public async Task<IEnumerable<ReviewDto>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = GenerateBase64Key(request.QueryOptions);

            var cachedArticles = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedArticles))
            {
                return JsonSerializer.Deserialize<IEnumerable<ReviewDto>>(cachedArticles);
            }

            IQueryable<ReviewDto> articles = _reviewRepository.GetQueryable();

            if (request.QueryOptions.Filter != null)
            {
                articles = request.QueryOptions.Filter.ApplyTo(articles, new ODataQuerySettings()) as IQueryable<ReviewDto>;
            }

            if (request.QueryOptions.OrderBy != null)
            {
                articles = request.QueryOptions.OrderBy.ApplyTo(articles, new ODataQuerySettings()) as IQueryable<ReviewDto>;
            }

            if (request.QueryOptions.Skip != null)
            {
                articles = articles.Skip((int)request.QueryOptions.Skip.Value);
            }

            if (request.QueryOptions.Top != null)
            {
                articles = articles.Take((int)request.QueryOptions.Top.Value);
            }

            var items = await Task.Run(() => articles.ToList(), cancellationToken);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(articles), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            }, cancellationToken);

            return items;
        }

        private string GenerateBase64Key(ODataQueryOptions<ReviewDto> queryOptions)
        {
            var keyBuilder = new StringBuilder();
            if (queryOptions.Filter != null) keyBuilder.Append($"Filter_{queryOptions.Filter.RawValue}_");
            if (queryOptions.OrderBy != null) keyBuilder.Append($"OrderBy_{queryOptions.OrderBy.RawValue}_");
            if (queryOptions.Skip != null) keyBuilder.Append($"Skip_{queryOptions.Skip.Value}_");
            if (queryOptions.Top != null) keyBuilder.Append($"Top_{queryOptions.Top.Value}_");

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(keyBuilder.ToString());
            return Convert.ToBase64String(plainTextBytes).Replace("/", "_").Replace("+", "-");
        }
    }
}
