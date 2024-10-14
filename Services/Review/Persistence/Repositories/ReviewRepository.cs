using Application.Common.CurrentUser;
using Domain.Dtos;
using Domain.Entities;
using Domain.Repositories;
using MongoDB.Driver;
using Persistence.Documents;

namespace Persistence.Repositories
{
    public class ReviewRepository : BaseRepository<ReviewDocument, Review>, IReviewRepository
    {
        public ReviewRepository(IMongoClient mongoClient, ICurrentUser currentUser) : base(mongoClient, currentUser) { }

        public IQueryable<ReviewDto> GetQueryable()
        {
            return _collection
                .AsQueryable()
                .Where(x => x.IsDeleted == false)
                .Select(x => new ReviewDto()
                {
                    ReviewContent = x.ReviewContent,
                    Reviewer = x.Reviewer,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    Id = x.Id,
                    IsDeleted = x.IsDeleted,
                    ArticleId = x.ArticleId,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                    Version = x.Version
                });
        }

        public async Task CreateAsync(Review review, CancellationToken cancellationToken)
        {
            await InsertOneWithVersioning((ReviewDocument)review, review, cancellationToken);
        }

        public async Task UpdateAsync(Review article, CancellationToken cancellationToken)
        {
            UpdateDefinition<ReviewDocument> updateDefinition = Builders<ReviewDocument>.Update
                    .Set(nameof(ReviewDocument.ReviewContent), article.ReviewContent)
                    .Set(nameof(ReviewDocument.Reviewer), article.Reviewer)
                    .Set(nameof(ReviewDocument.ArticleId), article.ArticleId)
                    .Set(nameof(ReviewDocument.IsDeleted), article.IsDeleted);

            await UpdateOneWithVersioning(article, updateDefinition, cancellationToken: cancellationToken);
        }

        public async Task<Review> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            FilterDefinition<ReviewDocument> filter = Builders<ReviewDocument>.Filter.Eq(x => x.IsDeleted, false);

            filter &= Builders<ReviewDocument>.Filter.Eq(x => x.Id, id);

            return await (await _collection.FindAsync(filter, cancellationToken: cancellationToken)).SingleOrDefaultAsync(cancellationToken);
        }
    }
}