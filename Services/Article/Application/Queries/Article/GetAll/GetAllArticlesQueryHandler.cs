using Domain.Dtos;
using Domain.Repositories;
using MediatR;

namespace Application.Queries.Article.GetAll
{
    public class GetAllArticlesQueryHandler : IRequestHandler<GetAllArticlesQuery, IQueryable<ArticleDto>>
    {
        private readonly IArticleRepository _articleRepository;

        public GetAllArticlesQueryHandler(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public Task<IQueryable<ArticleDto>> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<ArticleDto> articles = _articleRepository.GetQueryable();

            return Task.FromResult(articles); // IQueryable döndür
        }
    }
}
