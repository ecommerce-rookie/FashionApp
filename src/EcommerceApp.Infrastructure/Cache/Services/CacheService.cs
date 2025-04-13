using Domain.Models.Common;
using Infrastructure.Cache;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Infrastructure.Cache.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDatabase _cache;
        private readonly DefaultSystem _default;

        private JsonSerializerSettings jsonOpt = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };


        public CacheService(IConnectionMultiplexer connectionMultiplexer, IOptions<DefaultSystem> options)
        {
            _cache = connectionMultiplexer.GetDatabase();
            _default = options.Value;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var cacheValue = await _cache.StringGetAsync(key);

            // If cache has value, return cache data
            if (!cacheValue.IsNullOrEmpty)
            {
                return JsonConvert.DeserializeObject<T>(cacheValue!)!;
            }

            return default!;
        }

        public async Task<T> GetAsync<T>(string type, string key)
        {
            var fullKey = $"{type}:{key}";

            var cacheValue = await _cache.StringGetAsync(fullKey);

            // If cache has value, return cache data
            if (!cacheValue.IsNullOrEmpty)
            {
                return JsonConvert.DeserializeObject<T>(cacheValue!)!;
            }

            return default!;
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }

        public async Task RemoveAsync(string type, string key)
        {
            var fullKey = $"{type}:{key}";

            await _cache.KeyDeleteAsync(fullKey);
        }

        public async Task SetAsync<T>(string key, T value)
        {

            var jsonOpt = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            await _cache.StringSetAsync(key, JsonConvert.SerializeObject(value, jsonOpt), TimeSpan.FromMinutes(_default.CacheTime));

        }

        public async Task SetAsync<T>(string type, string key, T value)
        {
            var fullKey = $"{type}:{key}";

            await _cache.StringSetAsync(fullKey, JsonConvert.SerializeObject(value, jsonOpt), TimeSpan.FromMinutes(_default.CacheTime));

        }

        public async Task SetAsync<T>(string type, string key, T value, int timeCache)
        {
            var fullKey = $"{type}:{key}";


            var jsonOpt = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            await _cache.StringSetAsync(fullKey, JsonConvert.SerializeObject(value, jsonOpt), TimeSpan.FromMinutes(timeCache));

        }

        public async Task RemoveTypeAsync(string type)
        {
            var keys = await ScanKeysByPrefixAsync(type);

            const int batchSize = 20;

            foreach (var batch in keys.Chunk(batchSize))
            {
                await _cache.KeyDeleteAsync(batch.ToArray());
            }
        }


        private async Task<IEnumerable<RedisKey>> ScanKeysByPrefixAsync(string prefix)
        {
            var endpoints = _cache.Multiplexer.GetEndPoints();
            var server = _cache.Multiplexer.GetServer(endpoints.First());

            var keys = new List<RedisKey>();

            // pattern format: "type:*"
            var pattern = $"{prefix}:*";

            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                keys.Add(key);
            }

            return keys;
        }

    }
}
