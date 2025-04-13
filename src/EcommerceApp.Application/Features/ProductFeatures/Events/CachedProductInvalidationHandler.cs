using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Events;
using Infrastructure.Cache;
using MediatR;

namespace Application.Features.ProductFeatures.Events;

public class CachedProductInvalidationHandler : INotificationHandler<ModifiedProductEvent>
{
    private readonly ICacheService _cacheService;

    public CachedProductInvalidationHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(ModifiedProductEvent notification, CancellationToken cancellationToken)
    {
        var key = nameof(Product);

        await InvalidateCache(key);
    }

    private async Task InvalidateCache(string key) => await _cacheService.RemoveTypeAsync(key);
}