using Application.Features.CategoryFeatures.Models;
using Application.Features.ProductFeatures.Commands;
using Application.Features.ProductFeatures.Models;
using Application.Features.ProductFeatures.Queries;
using Asp.Versioning;
using Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/products")]
    [ApiController]
    [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public class ProductController : Controller
    {
        private readonly ISender _sender;

        public ProductController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(APIResponse<PagedList<ProductPreviewResponseModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts([FromQuery] GetListProductQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            Response.Headers.Append("X-Total-Count", JsonConvert.SerializeObject(result.Metadata));

            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<APIResponse<ProductResponseModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProduct(Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetProductQuery()
            {
                Id = id
            }, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<APIResponse<Guid>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            var result = await _sender.Send(command, cancellationToken);
            
            return StatusCode((int)result.Status, result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<APIResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeleteProductCommand()
            {
                Id = id
            }, cancellationToken);
         
            return StatusCode((int)result.Status, result);
        }

    }
}
