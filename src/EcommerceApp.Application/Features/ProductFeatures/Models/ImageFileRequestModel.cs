using Microsoft.AspNetCore.Http;

namespace Application.Features.ProductFeatures.Models
{
    public class ImageFileRequestModel
    {
        public IFormFile File { get; set; } = null!;
        public int OrderNumber { get; set; }
    }
}
