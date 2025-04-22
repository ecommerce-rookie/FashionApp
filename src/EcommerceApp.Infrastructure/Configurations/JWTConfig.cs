using Domain.Models.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configurations
{
    public static class JWTConfig
    {
        public static void AddJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var openIdSetting = configuration.GetSection(nameof(OpenIdSetting)).Get<OpenIdSetting>() ?? throw new Exception("OpenId is not config!");

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
            {
                options.Authority = openIdSetting.Authority;
                options.Audience = "api";
                options.RequireHttpsMetadata = false;
            });

        }
    }
}
