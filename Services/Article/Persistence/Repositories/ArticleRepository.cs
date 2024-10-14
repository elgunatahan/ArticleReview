using Application.Common.CurrentUser;
using Domain.Dtos;
using Domain.Entities;
using Domain.Repositories;
using MongoDB.Driver;
using Persistence.Documents;

namespace Persistence.Repositories
{
    public class ArticleRepository : BaseRepository<ArticleDocument, Article>, IArticleRepository
    {
        public ArticleRepository(IMongoClient mongoClient, ICurrentUser currentUser) : base(mongoClient, currentUser) { }

        public IQueryable<ArticleDto> GetQueryable()
        {
            return _collection
                .AsQueryable()
                .Where(x => x.IsDeleted == false)
                .Select(x => new ArticleDto()
                {
                    ArticleContent = x.ArticleContent,
                    Author = x.Author,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    Id = x.Id,
                    IsDeleted = x.IsDeleted,
                    PublishDate = x.PublishDate,
                    StarCount = x.StarCount,
                    Title = x.Title,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                    Version = x.Version
                });
        }
            
        public async Task CreateAsync(Article article, CancellationToken cancellationToken)
        {
            await InsertOneWithVersioning((ArticleDocument)article, article, cancellationToken);
        }

        public async Task UpdateAsync(Article article, CancellationToken cancellationToken)
        {
            UpdateDefinition<ArticleDocument> updateDefinition = Builders<ArticleDocument>.Update
                    .Set(nameof(ArticleDocument.ArticleContent), article.ArticleContent)
                    .Set(nameof(ArticleDocument.Author), article.Author)
                    .Set(nameof(ArticleDocument.PublishDate), article.PublishDate)
                    .Set(nameof(ArticleDocument.StarCount), article.StarCount)
                    .Set(nameof(ArticleDocument.Title), article.Title)
                    .Set(nameof(ArticleDocument.IsDeleted), article.IsDeleted);

            await UpdateOneWithVersioning(article, updateDefinition, cancellationToken: cancellationToken);
        }

        public async Task<Article> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            FilterDefinition<ArticleDocument> filter = Builders<ArticleDocument>.Filter.Eq(x => x.IsDeleted, false);

            filter &= Builders<ArticleDocument>.Filter.Eq(x => x.Id, id);

            return await (await _collection.FindAsync(filter, cancellationToken: cancellationToken)).SingleOrDefaultAsync(cancellationToken);
        }
    }
}