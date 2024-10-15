using MediatR;

namespace Application.Queries.Review.GetById
{

    public class GetReviewByIdQuery : IRequest<GetReviewByIdRepresentation>
    {
        public Guid Id { get; set; }
    }
}