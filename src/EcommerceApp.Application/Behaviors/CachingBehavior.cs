using Domain.Models.Common;
using Infrastructure.Cache;
using Infrastructure.Cache.Attributes;
using Infrastructure.Shared.Helpers;
using MediatR;
using System.Collections;
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

            // Check if error occurred or response.Data is empty
            if ((response is APIResponse apiResponse && apiResponse.IsSuccess && apiResponse.Data != null) ||
            (response is IEnumerable enumerables && enumerables.Cast<dynamic>().Any()))
            {
                // Set the cache with the response
                await _cache.SetAsync(cacheAttribute.Key, key, response, cacheAttribute.ExpirationTime);
            }

            return response;
        }
    }
}
