using Domain.Models.Settings;
using Infrastructure.HttpClients.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.HttpClients
{
    public static class HttpClientConfig
    {
        public static void AddHttpService(this WebApplicationBuilder builder)
        {
            var openIdSettings = builder.Configuration.GetSection(nameof(OpenIdSetting)).Get<OpenIdSetting>() 
                ?? throw new Exception("OpenId is not config!");

            builder.Services.AddHttpClient<IHttpService, HttpService>(client =>
            {
                client.BaseAddress = new Uri(openIdSettings.Authority);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            });
        }
    }
}
