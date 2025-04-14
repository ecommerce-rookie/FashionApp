using Domain.Aggregates.UserAggregate.Events;
using Infrastructure.ProducerTasks.CloudTaskProducers;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace Application.Features.UserFeatures.Events
{
    internal class UpdatedImageUserHandler : INotificationHandler<ModifedUserEvent>
    {
        private readonly ICloudTaskProducer _cloudTaskProducer;

        public UpdatedImageUserHandler(ICloudTaskProducer cloudTaskProducer)
        {
            _cloudTaskProducer = cloudTaskProducer;
        }

        public async Task Handle(ModifedUserEvent notification, CancellationToken cancellationToken)
        {
            // Add delete image on cloudinary task to the queue
            if (notification.Avatar.IsNullOrEmpty())
            {
                return;
            }

            _cloudTaskProducer.AddDeleteImageOnCloudinary(new List<string>()
            {
                notification.Avatar!
            });

            await Task.CompletedTask;
        }

    }
}
