using Application.Features.CategoryFeatures.Commands;
using Application.Features.CategoryFeatures.Queries;
using Application.Features.ProductFeatures.Queries;
using Asp.Versioning;
using Infrastructure.ProducerTasks.CloudTaskProducers;
using Infrastructure.ProducerTasks.EmailTaskProducers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/products")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly ISender _sender;

        public ProductController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetProducts([FromQuery] GetProductQuery query, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(query, cancellationToken);
            return Ok(result);
        }

        

    }
}
