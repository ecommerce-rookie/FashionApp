using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using static Infrastructure.Storage.Cloudinary.Internals.CloudinaryOptions;

namespace Infrastructure.Storage
{
    public interface IStorageService
    {
        Task<UploadResult> UploadImage(IFormFile file, ImageFolder folder, ImageFormat format, string? fileName);
        Task<UploadResult> UploadImage(byte[] file, ImageFolder folder, ImageFormat format, string? fileName);
		Task<Dictionary<string, string>> DeleteImages(IEnumerable<string> nameImages);
    }
}
