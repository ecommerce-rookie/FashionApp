using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Infrastructure.DocumentApi.swagger
{
    public class SwaggerGenConfig : IConfigureNamedOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public SwaggerGenConfig(IApiVersionDescriptionProvider provider) => _provider = provider;

        public void Configure(string? name, SwaggerGenOptions options)
        {
            Configure(options);
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = $"Ecommerce v{description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Description = "API for Ecommerce",
                    Contact = new OpenApiContact
                    {
                        Name = "Le Huy (Max)",
                        Email = "tongtranlehuy119@gmail.com",
                        Url = new("https://max-h.vercel.app")
                    },
                    License = new() { Name = "MIT", Url = new("https://opensource.org/licenses/MIT") }
                });
            }
        }
    }
}
