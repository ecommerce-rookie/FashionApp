namespace StoreFront.Domain.Models.ProductModels.Request
{
    public class ImageFileRequestModel
    {
        public IFormFile File { get; set; } = null!;
        public int OrderNumber { get; set; }
    }
}
