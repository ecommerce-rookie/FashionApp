using Application.Features.CategoryFeatures.Commands;
using Application.Features.CategoryFeatures.Queries;
using Application.Features.ProductFeatures.Queries;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly ISender _sender;

        public ProductController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories([FromQuery] GetcategoryQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> GetCategories(int id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeleteCategoryCommand()
            {
                Id = id
            }, cancellationToken);

            return Ok(result);
        }

    }
}
