using Domain.Entities;
using Domain.ValueObjects;
using Persistence.Interfaces;

namespace Persistence.Documents
{
    public class ReviewDocument: IDocument
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public Guid ArticleId { get; set; }
        public string Reviewer { get; set; }
        public string ReviewContent { get; set; }

        public static implicit operator Review(ReviewDocument document)
        {
            if (document == null)
                return null;

            Review review = new Review(
                reviewContent: document.ReviewContent,
                reviewer: document.Reviewer,
                articleId: document.ArticleId,

                isDeleted: document.IsDeleted,
                identity: new IdentityValueObject(document.Id, document.Version),
                audit: new AuditValueObject(document.CreatedBy, document.CreatedAt, document.UpdatedBy, document.UpdatedAt));

            return review;
        }

        public static explicit operator ReviewDocument(Review review)
        {
            if (review == null)
                return null;

            return new ReviewDocument()
            {
                Id = review.IdentityObject.Id,
                Version = review.IdentityObject.Version,
                CreatedBy = review.Audit?.CreatedBy,
                UpdatedBy = review.Audit?.UpdatedBy,
                IsDeleted = review.IsDeleted,

                ReviewContent = review.ReviewContent,
                Reviewer = review.Reviewer,
                ArticleId = review.ArticleId
            };
        }
    }
}
