using Domain.Constants.Common;
using Domain.Models.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.Configurations
{
    public static class JWTConfig
    {
        public static void AddJWT(this IServiceCollection services, IConfiguration configuration)
        {
            //var jwtSettings = configuration.GetSection(nameof(JWTSetting)).Get<JWTSetting>();
            var openIdSetting = configuration.GetSection(nameof(OpenIdSetting)).Get<OpenIdSetting>();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = "oidc";
            })
                .AddJwtBearer(options =>
            {
                options.Authority = "https://localhost:5001";
                options.Audience = "ecommerce-api";
                options.RequireHttpsMetadata = false;
            });
            //.AddOpenIdConnect("oidc", options =>
            //{
            //    options.Authority = openIdSetting?.Authority;
            //    options.ClientId = openIdSetting?.ClientId;
            //    options.ClientSecret = openIdSetting?.ClientSecret;
            //    options.ResponseType = "code";

            //    options.SaveTokens = true;
            //    options.Scope.Add("openid");
            //    options.Scope.Add("profile");
            //    options.Scope.Add("offline_access");
            //    options.Scope.Add(AuthScope.Read);

            //    // Quan trọng: trả về đúng swagger sau khi login
            //    //options.Events = new OpenIdConnectEvents
            //    //{
            //    //    OnRedirectToIdentityProvider = context =>
            //    //    {
            //    //        context.Properties.RedirectUri = "/swagger";
            //    //        return Task.CompletedTask;
            //    //    }
            //    //};
            //    options.CallbackPath = "/swagger/index.html";
            //});

        }
    }
}
