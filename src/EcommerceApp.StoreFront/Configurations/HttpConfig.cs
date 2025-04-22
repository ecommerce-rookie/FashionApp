using Refit;
using StoreFront.Application.Delegates;
using StoreFront.Application.Helpers;
using StoreFront.Domain.Constants;
using StoreFront.Domain.Models.Settings;

namespace StoreFront.Configurations
{
    public static class HttpConfig
    {
        public static void AddHttpConfig(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<LoggingDelegate>();

            services.AddTransient<AuthorizeDelegate>();

            var clientSetting = config.GetSection(nameof(ClientSetting)).Get<ClientSetting>() ?? throw new Exception("Not Setting client.");

            foreach (var type in TypeServiceConstants.TypeServices)
            {
                services.AddRefitClient(type, new()
                {
                    CollectionFormat = CollectionFormat.Multi,
                    ContentSerializer = new SystemTextJsonContentSerializer(new()
                    {
                        PropertyNamingPolicy = JsonHelper.GetJsonNamingPolicy(clientSetting.NamingPolicy),
                        WriteIndented = true
                    })
                })
                    .ConfigureHttpClient(c => c.BaseAddress = new(clientSetting.ApiEndpoint))
                    .AddHttpMessageHandler<LoggingDelegate>()
                    .AddHttpMessageHandler<AuthorizeDelegate>();
            }
        }
    }
}
