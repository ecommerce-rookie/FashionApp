using Application.Features.ProductFeatures.Commands;
using Application.Features.ProductFeatures.Models;
using Application.Features.ProductFeatures.Queries;
using Asp.Versioning;
using Domain.Constants;
using Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

            Response.Headers.Append(SystemConstant.HeaderPagination, JsonConvert.SerializeObject(result.Metadata));

            return Ok(result);
        }

        [HttpGet("recommend")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProductPreviewResponseModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecommendProducts([FromQuery] GetRecommendProductQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);
         
            return Ok(result);
        }

        [HttpGet("best-seller")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<ProductPreviewResponseModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBestSellerProducts([FromQuery] GetBestSellerProductQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpGet("{slug}")]
        [ProducesResponseType(typeof(APIResponse<APIResponse<ProductResponseModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProduct(string slug, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetProductQuery()
            {
                Slug = slug
            }, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPut("{slug}")]
        [ProducesResponseType(typeof(APIResponse<APIResponse<Guid>>), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> UpdateProduct([FromRoute] string slug, [FromForm] UpdateProductCommand command, CancellationToken cancellationToken)
        {
            command.Slug = slug;
            var result = await _sender.Send(command, cancellationToken);
            
            return StatusCode((int)result.Status, result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<APIResponse>), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, [FromQuery] bool? isHard, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeleteProductCommand()
            {
                Id = id,
                Hard = isHard
            }, cancellationToken);
         
            return StatusCode((int)result.Status, result);
        }

    }
}
