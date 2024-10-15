using Application.Commands.Review.Delete;
using Application.Queries.Review.GetAll;
using Application.Queries.Review.GetById;
using Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using ReviewApi.Common;
using ReviewApi.Models.Requests;

namespace ReviewApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultExceptionDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(DefaultExceptionDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(DefaultExceptionDto))]
        [ProducesResponseType(StatusCodes.Status502BadGateway, Type = typeof(DefaultExceptionDto))]
        [Authorize(Roles = "ADMIN,ONLYREVIEWAPI")]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest request, CancellationToken cancellationToken)
        {
            var representation = await _mediator.Send(request.ToCommand(), cancellationToken);

            return Created("", new { id = representation.Id });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultExceptionDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(DefaultExceptionDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(DefaultExceptionDto))]
        [ProducesResponseType(StatusCodes.Status502BadGateway, Type = typeof(DefaultExceptionDto))]
        [Authorize(Roles = "ADMIN,ONLYREVIEWAPI")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateReviewRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(request.ToCommand(id), cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultExceptionDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(DefaultExceptionDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(DefaultExceptionDto))]
        [Authorize(Roles = "ADMIN,ONLYREVIEWAPI")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteReviewCommand()
            {
                Id = id
            }, cancellationToken);

            return NoContent();
        }



        [HttpGet]
        [EnableQuery(MaxTop = 100, PageSize = 100)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(DefaultExceptionDto))]
        [Authorize(Roles = "ADMIN,MEMBER,ONLYREVIEWAPI")]
        public async Task<IActionResult> Get(ODataQueryOptions<ReviewDto> queryOptions, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllReviewsQuery()
            {
                QueryOptions = queryOptions
            }, cancellationToken);

            return Ok(result);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetReviewByIdRepresentation))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(DefaultExceptionDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(DefaultExceptionDto))]
        [Authorize(Roles = "ADMIN,MEMBER,ONLYREVIEWAPI")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetReviewByIdQuery()
            {
                Id = id
            }, cancellationToken);

            return Ok(result);
        }
    }
}
