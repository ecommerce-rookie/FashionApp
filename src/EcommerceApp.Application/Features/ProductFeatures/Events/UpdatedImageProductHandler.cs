using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Events;
using Infrastructure.Cache;
using Infrastructure.ProducerTasks.CloudTaskProducers;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.ProductFeatures.Events
{
    public class UpdatedImageProductHandler : INotificationHandler<ModifiedProductEvent>
    {
        private readonly ICloudTaskProducer _cloudTaskProducer;
        private readonly ICacheService _cacheService;

        public UpdatedImageProductHandler(ICloudTaskProducer cloudTaskProducer, ICacheService cacheService)
        {
            _cloudTaskProducer = cloudTaskProducer;
            _cacheService = cacheService;
        }

        public async Task Handle(ModifiedProductEvent notification, CancellationToken cancellationToken)
        {
            // Invalidate cache
            var key = nameof(Product);

            await InvalidateCache(key);

            // Add delete image on cloudinary task to the queue
            if (notification.Images.IsNullOrEmpty())
            {
                return;
            }

            _cloudTaskProducer.AddDeleteImageOnCloudinary(notification.Images!.ToList());

        }

        private async Task InvalidateCache(string key) => await _cacheService.RemoveTypeAsync(key);
    }
}
