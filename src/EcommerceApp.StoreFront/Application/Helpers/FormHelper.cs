using Refit;

namespace StoreFront.Application.Helpers
{
    public static class FormHelper
    {
        public static StreamPart CreateStreamPart(this IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be null or empty.", nameof(file));
            }
            
            var stream = file.OpenReadStream();
            var streamPart = new StreamPart(stream, file.FileName, file.ContentType);
            
            return streamPart;
        }
    }
}
