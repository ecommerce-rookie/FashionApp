using Domain.Constants.Common;
using Domain.Models.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Infrastructure.DocumentApi.swagger
{
    public static class SwaggerConfig
    {
        public static void AddSwagger(this IServiceCollection services, IConfiguration config)
        {
            var openIdSettings = config.GetSection(nameof(OpenIdSetting)).Get<OpenIdSetting>() ?? throw new Exception("OpenId is not config!");

            services.ConfigureOptions<SwaggerGenConfig>();
            services.AddSwaggerGen(swagger =>
            {
                // Add authorization with Bearer token
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token using the Bearer scheme (\"bearer {token}\")",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "Security",
                    Scheme = "Bearer"
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

                // Add authorization with OAuth2
                swagger.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri($"{openIdSettings.Authority}/connect/token"),
                            AuthorizationUrl = new Uri($"{openIdSettings.Authority}/connect/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "profile", "Get Profile User" },
                                { "openid", "Get Informations related to Authen User" },
                                { "offline_access", "Get Refresh Token" },
                                { "api", "Get All Informations For User" }
                            },
                            RefreshUrl = new Uri($"{openIdSettings.Authority}/connect/token"),
                        },
                    },
                    Description = "OAuth2 Authorization Code Flow"
                });

                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2",
                            }
                        },
                        new[] { "profile", "api", "openid" }
                    }
                });

            });
        }
    }
}
