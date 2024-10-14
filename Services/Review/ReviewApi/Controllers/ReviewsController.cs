using Application.Commands.Review.Delete;
using Application.Queries.Review.GetAll;
using Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using ReviewApi.Common;
using ReviewApi.Models.Requests;

namespace ReviewApi.Controllers
{
    //[ApiController]
    //[Route("api/v1/[controller]")]
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
        //[Authorize]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest request, CancellationToken cancellationToken)
        {
            var representation = await _mediator.Send(request.ToCommand(), cancellationToken);

            return Created("", new { id = representation.Id });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //[Authorize]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateReviewRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(request.ToCommand(id), cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(DefaultExceptionDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultExceptionDto))]
        //[Authorize]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteReviewCommand()
            {
                Id = id
            }, cancellationToken);

            return NoContent();
        }



        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get(ODataQueryOptions<ReviewDto> queryOptions, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllReviewsQuery()
            {
                QueryOptions = queryOptions
            }, cancellationToken);

            return Ok(result);
        }
    }
}
