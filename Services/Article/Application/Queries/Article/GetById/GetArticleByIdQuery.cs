using MediatR;

namespace Application.Queries.Article.GetById
{

    public class GetArticleByIdQuery : IRequest<GetArticleByIdRepresentation>
    {
        public Guid Id { get; set; }
    }
}