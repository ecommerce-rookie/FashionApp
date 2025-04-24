using Application.Features.CategoryFeatures.Commands;
using Application.Features.CategoryFeatures.Models;
using Application.Features.CategoryFeatures.Queries;
using Asp.Versioning;
using Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/categories")]
    [ApiController]
    [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public class CategoryController : Controller
    {
        private readonly ISender _sender;

        public CategoryController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<CategoryResponseModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategories([FromQuery] GetcategoryQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeleteCategoryCommand()
            {
                Id = id
            }, cancellationToken);

            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

    }
}
