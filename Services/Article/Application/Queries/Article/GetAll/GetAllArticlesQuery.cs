using Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.OData.Query;

namespace Application.Queries.Article.GetAll
{

    public class GetAllArticlesQuery : IRequest<IEnumerable<ArticleDto>>
    {
        public ODataQueryOptions<ArticleDto> QueryOptions {  get; set; }
    }
}