using Application.Commands.Review.Create;

namespace ReviewApi.Models.Requests
{
    public class CreateReviewRequest
    {
        public Guid ArticleId { get; set; }
        public string Reviewer { get; set; }
        public string ReviewContent { get; set; }

        public CreateReviewCommand ToCommand()
        {
            return new CreateReviewCommand()
            {
                ArticleId = ArticleId,
                Reviewer = Reviewer,
                ReviewContent = ReviewContent
            };
        }
    }
}
