using Domain.Aggregates.FeedbackAggregate.Entities;
using Domain.Aggregates.FeedbackAggregate.Events;
using Infrastructure.Cache;
using MediatR;

namespace Application.Features.FeedbackFeatures.Events
{
    public class CachedFeedbackInvalidationHandler : INotificationHandler<ModifiedFeedbackEvent>
    {
        private readonly ICacheService _cacheService;

        public CachedFeedbackInvalidationHandler(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task Handle(ModifiedFeedbackEvent notification, CancellationToken cancellationToken)
        {
            var key = nameof(Feedback);

            await InvalidateCache(key);
        }

        private async Task InvalidateCache(string key) => await _cacheService.RemoveTypeAsync(key);
    }
}
