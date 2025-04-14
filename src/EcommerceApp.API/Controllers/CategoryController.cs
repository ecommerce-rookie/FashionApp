using Application.Features.CategoryFeatures.Commands;
using Application.Features.CategoryFeatures.Queries;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/categories")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ISender _sender;

        public CategoryController(ISender sender)
        {
            _sender = sender;
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
