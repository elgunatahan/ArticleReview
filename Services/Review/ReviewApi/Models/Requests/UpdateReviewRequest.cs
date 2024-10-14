using Application.Commands.Review.Update;

namespace ReviewApi.Models.Requests
{
    public class UpdateReviewRequest
    {
        public Guid ArticleId { get; set; }
        public string Reviewer { get; set; }
        public string ReviewContent { get; set; }

        public UpdateReviewCommand ToCommand(Guid id)
        {
            return new UpdateReviewCommand()
            {
                Id = id,
                ArticleId = ArticleId,
                Reviewer = Reviewer,
                ReviewContent = ReviewContent
            };
        }
    }
}
