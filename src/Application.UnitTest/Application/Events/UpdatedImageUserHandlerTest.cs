using Application.Features.UserFeatures.Events;
using Domain.Aggregates.UserAggregate.Events;
using Infrastructure.ProducerTasks.CloudTaskProducers;
using Moq;

namespace Application.UnitTest.Application.Events
{
    public class UpdatedImageUserHandlerTest
    {
        private readonly Mock<ICloudTaskProducer> _cloudTaskProducerMock;
        private readonly UpdatedImageUserHandler _handler;

        public UpdatedImageUserHandlerTest()
        {
            _cloudTaskProducerMock = new Mock<ICloudTaskProducer>();
            _handler = new UpdatedImageUserHandler(_cloudTaskProducerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_AddTask_When_AvatarIsNotEmpty()
        {
            // Arrange
            var notification = new ModifedUserEvent
            {
                Avatar = "avatarUrl"
            };

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _cloudTaskProducerMock.Verify(c => c.AddDeleteImageOnCloudinary(It.IsAny<List<string>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Not_AddTask_When_AvatarIsEmpty()
        {
            // Arrange
            var notification = new ModifedUserEvent
            {
                Avatar = string.Empty
            };

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _cloudTaskProducerMock.Verify(c => c.AddDeleteImageOnCloudinary(It.IsAny<List<string>>()), Times.Never);
        }
    }
}
