using Application.Features.OrderFeatures.Commands;
using Application.Features.OrderFeatures.Models;
using Application.Features.OrderFeatures.Queries;
using Asp.Versioning;
using Domain.Constants;
using Domain.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/orders")]
    [ApiController]
    [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ISender _sender;

        public OrderController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(APIResponse<PagedList<OrderResponseModel>>), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> GetListOrder([FromQuery] GetListOrderQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);
            
            Response.Headers.Append(SystemConstant.HeaderPagination, JsonSerializer.Serialize(result.Metadata));

            return Ok(result);
        }

        [HttpPatch("{orderId}/status")]
        [ProducesResponseType(typeof(APIResponse<OrderResponseModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] Guid orderId, [FromBody] UpdateOrderStatusCommand command, 
            CancellationToken cancellationToken)
        {
            command.OrderId = orderId;
            var result = await _sender.Send(command, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(APIResponse<OrderResponseModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(APIResponse<OrderResponseModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] CheckoutCommand checkoutCommand, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(checkoutCommand, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

    }
}
