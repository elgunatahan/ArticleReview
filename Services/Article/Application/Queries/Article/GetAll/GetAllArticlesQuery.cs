using Domain.Dtos;
using MediatR;

namespace Application.Queries.Article.GetAll
{

    public class GetAllArticlesQuery : IRequest<IQueryable<ArticleDto>>
    {

    }
}