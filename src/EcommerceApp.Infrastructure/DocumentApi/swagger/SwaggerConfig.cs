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
            var openIdSettings = config.GetSection(nameof(OpenIdSetting)).Get<OpenIdSetting>();

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
                            TokenUrl = new Uri($"https://localhost:5001/connect/token"),
                            AuthorizationUrl = new Uri($"https://localhost:5001/connect/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                { AuthScope.Read, "Read Access to API" },
                                { AuthScope.Write, "Write Access to API" }
                            },
                            RefreshUrl = new Uri($"https://localhost:5001/connect/token"),
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
                        new[] { AuthScope.Read, AuthScope.Write }
                    }
                });

            });
        }
    }
}
