using Application.Exceptions;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Application.Queries.Article.GetById
{
    public class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, GetArticleByIdRepresentation>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IDistributedCache _cache;

        public GetArticleByIdQueryHandler(IArticleRepository articleRepository, IDistributedCache cache)
        {
            _articleRepository = articleRepository;
            _cache = cache;
        }

        public async Task<GetArticleByIdRepresentation> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
        {
            var cachedArticle = await _cache.GetStringAsync(request.Id.ToString(), cancellationToken);

            if (!string.IsNullOrEmpty(cachedArticle))
            {
                return JsonSerializer.Deserialize<GetArticleByIdRepresentation>(cachedArticle);
            }

            Domain.Entities.Article article = await _articleRepository.GetByIdAsync(request.Id, cancellationToken);

            if (article == null)
            {
                throw new ArticleNotFoundException(request.Id);
            }

            var representation = new GetArticleByIdRepresentation(article);

            await _cache.SetStringAsync(request.Id.ToString(), JsonSerializer.Serialize(representation), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            }, cancellationToken);

            return representation;
        }
    }
}
