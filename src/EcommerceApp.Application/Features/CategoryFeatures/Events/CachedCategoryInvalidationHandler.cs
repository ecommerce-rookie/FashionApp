using Domain.Aggregates.CategoryAggregate.Entities;
using Domain.Aggregates.CategoryAggregate.Events;
using Infrastructure.Cache;
using MediatR;

namespace Application.Features.CategoryFeatures.Events;

public class CachedCategoryInvalidationHandler : INotificationHandler<ModifiedCategoryEvent>
{
    private readonly ICacheService _cacheService;

    public CachedCategoryInvalidationHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(ModifiedCategoryEvent notification, CancellationToken cancellationToken)
    {
        var key = nameof(Category);

        await InvalidateCache(key);
    }

    private async Task InvalidateCache(string key) => await _cacheService.RemoveTypeAsync(key);
}