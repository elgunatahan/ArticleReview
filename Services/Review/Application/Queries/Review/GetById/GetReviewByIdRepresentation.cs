namespace Application.Queries.Review.GetById
{
    public class GetReviewByIdRepresentation
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

        public GetReviewByIdRepresentation()
        {

        }

        public GetReviewByIdRepresentation(Domain.Entities.Review review)
        {
            Id = review.IdentityObject.Id;
            Version = review.IdentityObject.Version;
            CreatedAt = review.Audit.CreatedAt.Value;
            CreatedBy = review.Audit.CreatedBy;
            UpdatedAt = review.Audit.UpdatedAt;
            UpdatedBy = review.Audit.UpdatedBy;

            ArticleId = review.ArticleId;
            Reviewer = review.Reviewer;
            ReviewContent = review.ReviewContent;
        }
    }
}
