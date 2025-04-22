using Application.Features.UserFeatures.Commands;
using Application.Features.UserFeatures.Models;
using Application.Features.UserFeatures.Queries;
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
    [Route("api/v{version:apiVersion}/users")]
    [ApiController]
    [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [Authorize]
    public class UserController : Controller
    {
        private readonly ISender _sender;

        public UserController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUser([FromForm] CreateUserCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPatch("{id:guid}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteUserCommand { Id = id };
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(APIResponse<PagedList<UserPreviewResponseModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListUser([FromQuery] GetListUserQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);

            Response.Headers.Append(SystemConstant.HeaderPagination, JsonConvert.SerializeObject(result.Metadata));

            return Ok(result);
        }

        [HttpGet("author")]
        [ProducesResponseType(typeof(APIResponse<AuthorResponseModel>), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> GetAuthor(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new GetAuthorQuery(), cancellationToken);

            return Ok(result);
        }

    }
}
