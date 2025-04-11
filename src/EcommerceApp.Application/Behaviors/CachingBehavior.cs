using Infrastructure.Cache;
using Infrastructure.Cache.Attributes;
using Infrastructure.Shared.Helpers;
using MediatR;
using System.Reflection;

namespace Application.Behaviors
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ICacheService _cache;

        public CachingBehavior(ICacheService cache)
        {
            _cache = cache;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Check if the request has a CacheAttribute
            var cacheAttribute = typeof(TRequest).GetCustomAttribute<CacheAttribute>();
            if (cacheAttribute == null)
            {
                return await next();
            }

            // Generate cache key
            var key = $"{NameHelper.GenerateName(request, new object[] { request })}";

            // Check if the cache has the response
            var cached = await _cache.GetAsync<TResponse>(key);
            if (cached is not null)
            {
                return cached;
            }

            // Call the next handler in the pipeline
            var response = await next();

            // If the response is not null, set it in the cache
            await _cache.SetAsync(cacheAttribute.Key, key, response, cacheAttribute.ExpirationTime);

            return response;
        }
    }
}
