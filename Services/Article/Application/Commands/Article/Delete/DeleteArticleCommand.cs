using MediatR;

namespace Application.Commands.Article.Delete
{
    public class DeleteArticleCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
