using Application.Commands.Article.Delete;
using Application.Queries.Article.GetAll;
using ArticleApi.Common;
using ArticleApi.Models.Requests;
using Domain.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace ArticleApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ArticlesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        //[Authorize]
        public async Task<IActionResult> Create([FromBody] CreateArticleRequest request, CancellationToken cancellationToken)
        {
            var representation = await _mediator.Send(request.ToCommand(), cancellationToken);

            return Created("",new { id = representation.Id });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //[Authorize]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateArticleRequest request, CancellationToken cancellationToken)
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
            await _mediator.Send(new DeleteArticleCommand()
            {
                Id = id
            }, cancellationToken);

            return NoContent();
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get(ODataQueryOptions<ArticleDto> queryOptions, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllArticlesQuery()
            {
                QueryOptions = queryOptions
            }, cancellationToken);

            return Ok(result);
        }
    }
}
