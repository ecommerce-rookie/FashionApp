using Infrastructure.Storage.Cloudinary.Services;
using Infrastructure.Storage.Cloudinary.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Storage
{
    public static class StorageConfig
    {
        public static void AddStorage(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IStorageService, CloudinaryService>();

            services.Configure<CloudinarySetting>(configuration.GetSection(nameof(CloudinarySetting)));

        }
    }
}
