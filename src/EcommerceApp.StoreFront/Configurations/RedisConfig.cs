using StackExchange.Redis;
using StoreFront.Domain.Models.Settings;

namespace StoreFront.Configurations
{
    public static class RedisConfig
    {
        public static void AddRedis(this WebApplicationBuilder builder)
        {
            var redisSetting = builder.Configuration.GetSection(nameof(RedisSetting)).Get<RedisSetting>() ?? throw new Exception($"{nameof(RedisSetting)} is not config!");

            var options = new ConfigurationOptions
            {
                ClientName = redisSetting.ClientName,
                EndPoints = { { redisSetting.Host, redisSetting.Port } },
                User = redisSetting.User,
                Password = redisSetting.Password,
                AbortOnConnectFail = false,
                Ssl = true
            };

            var multiplexer = ConnectionMultiplexer.Connect(options);
            builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        }
    }
}
