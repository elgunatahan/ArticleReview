using MediatR;

namespace Application.Commands.Article.Create
{
    public class CreateArticleCommand : IRequest<CreateArticleRepresentation>
    {
        public string ArticleContent { get; set; }
        public string Author { get; set; }
        public DateTime PublishDate { get; set; }
        public int StarCount { get; set; }
        public string Title { get; set; }
    }
}
