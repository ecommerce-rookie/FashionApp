using Domain.Constants.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace Infrastructure.DocumentApi
{
    public static class ScalarConfig
    {
        public static void UseScalar(this WebApplication app)
        {
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "/openapi/{documentName}.json";
                options.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers.Add(new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" });
                });
            });

            app.MapScalarApiReference(options =>
            {
                options.EndpointPathPrefix = "/api/{documentName}";
                options.WithOAuth2Authentication(opt =>
                {
                    opt.ClientId = "swagger-client";
                    opt.Scopes = new List<string>
                    {
                        { AuthScope.Read }
                    };
                });
                //{
                //    TokenUrl = new Uri($"https://localhost:5001/connect/token"),
                //    AuthorizationUrl = new Uri($"https://localhost:5001/connect/authorize"),
                //    Scopes = new Dictionary<string, string>
                //    {
                //        { AuthScope.Read, "Read Access to API" },
                //        { AuthScope.Write, "Write Access to API" }
                //    },
                //    RefreshUrl = new Uri($"https://localhost:5001/connect/token"),
                //});
            });
        }
    }
}
