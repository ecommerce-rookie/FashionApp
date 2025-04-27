using Application.Features.FeedbackFeatures.Commands;
using Application.Features.FeedbackFeatures.Models;
using Application.Features.FeedbackFeatures.Queries;
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
    [Route("api/v{version:apiVersion}/feedbacks")]
    [ApiController]
    [ProducesResponseType(typeof(APIResponse<ErrorValidation>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public class FeedbackController : Controller
    {
        private readonly ISender _sender;

        public FeedbackController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{productId}")]
        [ProducesResponseType(typeof(APIResponse<PagedList<FeedbackResponseModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeedbacks([FromRoute] Guid productId, [FromQuery] GetListFeedbackQuery query, CancellationToken cancellationToken)
        {
            query.ProductId = productId;

            var result = await _sender.Send(query, cancellationToken);

            Response.Headers.Append(SystemConstant.HeaderPagination, JsonConvert.SerializeObject(result.Metadata));

            return Ok(result);
        }

        [HttpPost("{productId}")]
        [ProducesResponseType(typeof(APIResponse<Guid>), StatusCodes.Status201Created)]
        [Authorize(PolicyType.Customer)]
        public async Task<IActionResult> CreateFeedback([FromRoute] Guid productId, [FromBody] CreateFeedbackCommand request, CancellationToken cancellationToken)
        {
            request.ProductId = productId;
            var result = await _sender.Send(request, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpPatch("{productId}")]
        [ProducesResponseType(typeof(APIResponse<FeedbackResponseModel>), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> UpdateFeedback([FromRoute] Guid productId, [FromBody] UpdateFeedbackCommand request, CancellationToken cancellationToken)
        {
            request.ProductId = productId;
            var result = await _sender.Send(request, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpDelete("{feedbackId}")]
        [ProducesResponseType(typeof(APIResponse<FeedbackResponseModel>), StatusCodes.Status200OK)]
        [Authorize]
        public async Task<IActionResult> DeleteFeedback([FromRoute] Guid feedbackId, [FromQuery] bool? isHard, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new DeleteFeedbackCommand() 
            { 
                Id = feedbackId,
                Hard = isHard
            }, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

        [HttpGet("{productId}/me")]
        [ProducesResponseType(typeof(APIResponse<FeedbackResponseModel>), StatusCodes.Status200OK)]
        [Authorize(PolicyType.Customer)]
        public async Task<IActionResult> GetFeedback([FromRoute] Guid productId, [FromQuery] GetFeedbackQuery query, CancellationToken cancellationToken)
        {
            query.ProductId = productId;
            
            var result = await _sender.Send(query, cancellationToken);

            return StatusCode((int)result.Status, result);
        }

    }
}
