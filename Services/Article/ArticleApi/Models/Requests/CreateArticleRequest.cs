using Application.Commands.Article.Create;

namespace ArticleApi.Models.Requests
{
    public class CreateArticleRequest
    {
        public string Author { get; set; }
        public string ArticleContent { get; set; }
        public DateTime PublishDate { get; set; }
        public int StarCount { get; set; }
        public string Title { get; set; }

        public CreateArticleCommand ToCommand()
        {
            return new CreateArticleCommand()
            {
                Author = Author,
                ArticleContent = ArticleContent,
                PublishDate = PublishDate,
                StarCount = StarCount,
                Title = Title
            };
        }
    }
}
