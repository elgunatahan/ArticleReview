using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid ArticleId { get; private set; }
        public string Reviewer { get; private set; }
        public string ReviewContent { get; private set; }
        public bool IsDeleted { get; private set; }

        public Review(
            Guid articleId,
            string reviewer,
            string reviewContent,
            bool isDeleted = false,
            IdentityValueObject identity = null,
            AuditValueObject audit = null) : base(identity)
        {
            ArticleId = articleId;
            Reviewer = reviewer;
            ReviewContent = reviewContent;

            IsDeleted = isDeleted;
            Audit = audit;

            if (ArticleId == Guid.Empty)
            {
                throw new ValidationException("ArticleId is empty guid");
            }

            if (string.IsNullOrWhiteSpace(Reviewer))
            {
                throw new ValidationException("Reviewer is null or white space");
            }

            if (string.IsNullOrWhiteSpace(ReviewContent))
            {
                throw new ValidationException("ReviewContent is null or white space");
            }
        }

        public void Update(
            Guid articleId,
            string reviewer,
            string reviewContent
            )
        {
            ArticleId = articleId;
            Reviewer = reviewer;
            ReviewContent = reviewContent;

            if (ArticleId == Guid.Empty)
            {
                throw new ValidationException("ArticleId is empty guid");
            }

            if (string.IsNullOrWhiteSpace(Reviewer))
            {
                throw new ValidationException("Reviewer is null or white space");
            }

            if (string.IsNullOrWhiteSpace(ReviewContent))
            {
                throw new ValidationException("ReviewContent is null or white space");
            }
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
