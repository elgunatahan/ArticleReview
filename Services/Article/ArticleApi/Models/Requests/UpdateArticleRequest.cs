using Application.Commands.Article.Update;

namespace ArticleApi.Models.Requests
{
    public class UpdateArticleRequest
    {
        public string Author { get; set; }
        public string ArticleContent { get; set; }
        public DateTime PublishDate { get; set; }
        public int StarCount { get; set; }
        public string Title { get; set; }

        public UpdateArticleCommand ToCommand(Guid id)
        {
            return new UpdateArticleCommand()
            {
                Id = id,
                Author = Author,
                ArticleContent = ArticleContent,
                PublishDate = PublishDate,
                StarCount = StarCount,
                Title = Title
            };
        }
    }
}
