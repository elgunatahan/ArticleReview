using Domain.Dtos;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IArticleRepository 
    {
        IQueryable<ArticleDto> GetQueryable();

        Task CreateAsync(Article article, CancellationToken cancellationToken);

        Task UpdateAsync(Article article, CancellationToken cancellationToken);

        Task<Article> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}