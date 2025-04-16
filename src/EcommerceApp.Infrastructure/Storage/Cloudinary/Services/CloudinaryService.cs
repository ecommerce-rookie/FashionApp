using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Models.Common;
using Infrastructure.Shared.Extensions;
using Infrastructure.Storage.Cloudinary.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using static Infrastructure.Storage.Cloudinary.Internals.CloudinaryOptions;

namespace Infrastructure.Storage.Cloudinary.Services
{
    public class CloudinaryService : IStorageService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;

        public CloudinaryService(IOptions<CloudinarySetting> config, IOptions<DefaultSystem> options)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret);

            _cloudinary = new CloudinaryDotNet.Cloudinary(account);
        }

        public async Task<UploadResult> UploadImage(IFormFile file, ImageFolder folder, ImageFormat format, string? fileName)
        {
            var name = string.IsNullOrEmpty(fileName) ? $"{DateTime.Now.Ticks}" : $"{fileName}_${DateTime.Now.Ticks}";
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder.GetDescription(),
                Format = format.ToString(),
                PublicId = name,
                PublicIdPrefix = "/images"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            return result;
        }

        public async Task<UploadResult> UploadImage(byte[] file, ImageFolder folder, ImageFormat format, string? fileName)
        {
            var name = string.IsNullOrEmpty(fileName) ? $"{DateTime.Now.Ticks}" : $"{fileName}_${DateTime.Now.Ticks}";
            await using var stream = new MemoryStream(file);
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription("image upload", stream),
                Folder = folder.GetDescription(),
                Format = format.ToString(),
                PublicId = name,
                PublicIdPrefix = "/images"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            return result;
        }

        public async Task<Dictionary<string, string>> DeleteImages(IEnumerable<string> nameImages)
        {
            var deleteParams = new DelResParams()
            {
                PublicIds = nameImages.ToList(),
                Type = "upload",
                ResourceType = ResourceType.Image
            };

            var result = await _cloudinary.DeleteResourcesAsync(deleteParams);

            return result.Deleted;
        }
    }
}
