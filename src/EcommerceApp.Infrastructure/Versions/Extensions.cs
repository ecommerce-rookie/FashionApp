using Asp.Versioning;
using Domain.Constants;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Versions
{
    public static class APIVersionConfig
    {
        public static void AddAPIVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1);
                opt.ReportApiVersions = true;
                opt.ApiVersionReader = ApiVersionReader.Combine(
                        new HeaderApiVersionReader(SystemConstant.HeaderApiVersion),
                        new QueryStringApiVersionReader(SystemConstant.HeaderApiVersion),
                        new UrlSegmentApiVersionReader()
                );
            }).AddApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'V";
                opt.SubstituteApiVersionInUrl = true;
            });
        }
    }
}
