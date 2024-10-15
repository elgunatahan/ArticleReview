using Domain.Dtos;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface IReviewRepository
    {
        IQueryable<ReviewDto> GetQueryable();

        Task CreateAsync(Review review, CancellationToken cancellationToken);

        Task UpdateAsync(Review review, CancellationToken cancellationToken);

        Task<Review> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}