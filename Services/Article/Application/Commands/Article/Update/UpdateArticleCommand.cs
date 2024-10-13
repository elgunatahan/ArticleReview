using MediatR;

namespace Application.Commands.Article.Update
{
    public class UpdateArticleCommand : IRequest
    {
        public Guid Id { get; set; }
        public string ArticleContent { get; set; }
        public string Author { get; set; }
        public DateTime PublishDate { get; set; }
        public int StarCount { get; set; }
        public string Title { get; set; }
    }
}
