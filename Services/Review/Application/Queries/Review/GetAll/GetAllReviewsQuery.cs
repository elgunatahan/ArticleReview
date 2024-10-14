using Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.OData.Query;

namespace Application.Queries.Review.GetAll
{

    public class GetAllReviewsQuery : IRequest<IEnumerable<ReviewDto>>
    {
        public ODataQueryOptions<ReviewDto> QueryOptions {  get; set; }
    }
}