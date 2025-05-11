using Application.Features.ProductFeatures.Events;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Events;
using Infrastructure.Cache;
using Infrastructure.ProducerTasks.CloudTaskProducers;
using Moq;

namespace Application.UnitTest.Application.Events
{
    public class UpdatedImageProductHandlerTest
    {
        private readonly Mock<ICloudTaskProducer> _cloudTaskProducerMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly UpdatedImageProductHandler _handler;

        public UpdatedImageProductHandlerTest()
        {
            _cloudTaskProducerMock = new Mock<ICloudTaskProducer>();
            _cacheServiceMock = new Mock<ICacheService>();
            _handler = new UpdatedImageProductHandler(_cloudTaskProducerMock.Object, _cacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_Should_InvalidateCache_And_AddTask_When_ImagesAreNotEmpty()
        {
            // Arrange
            var notification = new ModifiedProductEvent
            {
                Images = new List<string> { "url1", "url2" }
            };

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _cacheServiceMock.Verify(c => c.RemoveTypeAsync(nameof(Product)), Times.Once);
            _cloudTaskProducerMock.Verify(c => c.AddDeleteImageOnCloudinary(It.IsAny<List<string>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Not_AddTask_When_ImagesAreEmpty()
        {
            // Arrange
            var notification = new ModifiedProductEvent
            {
                Images = new List<string>()
            };

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _cloudTaskProducerMock.Verify(c => c.AddDeleteImageOnCloudinary(It.IsAny<List<string>>()), Times.Never);
        }
    }
}
