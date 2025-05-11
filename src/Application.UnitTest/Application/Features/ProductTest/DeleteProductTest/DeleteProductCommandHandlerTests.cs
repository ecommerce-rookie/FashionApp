using Application.Features.ProductFeatures.Commands;
using Application.Messages;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Aggregates.ProductAggregate.Events;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using MediatR;
using Moq;
using System.Net;

namespace Application.UnitTest.Application.Features.ProductTest.DeleteProductTest
{
    public class DeleteProductCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly DeleteProductCommandHandler _handler;

        public DeleteProductCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPublisher = new Mock<IPublisher>();

            _handler = new DeleteProductCommandHandler(
                _mockUnitOfWork.Object,
                _mockPublisher.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var command = new DeleteProductCommand
            {
                Id = Guid.NewGuid()
            };

            _mockUnitOfWork.Setup(uow => uow.ProductRepository.GetById(It.IsAny<Guid>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be(MessageCommon.NotFound);
            result.Data.Should().Be(command.Id);
        }

        [Fact]
        public async Task Handle_ShouldDeleteProductSoftly_WhenHardDeleteIsNotSet()
        {
            // Arrange
            var command = new DeleteProductCommand
            {
                Id = Guid.NewGuid(),
                Hard = false
            };

            var product = new Product(command.Id, "Product Name", 100, 90, "Description", ProductStatus.Available, 1, 10, new List<string> { "L", "M" }, Gender.Male, Guid.NewGuid());
            _mockUnitOfWork.Setup(uow => uow.ProductRepository.GetManageById(command.Id))
                .ReturnsAsync(product);

            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.DeleteSuccessfully);
            product.Id.Should().Be(command.Id);
            product.IsDeleted.Should().BeTrue();
            _mockUnitOfWork.Verify(uow => uow.ProductRepository.Delete(It.IsAny<Product>()), Times.Never);
            _mockPublisher.Verify(p => p.Publish(It.IsAny<ModifiedProductEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldDeleteProductHardly_WhenHardDeleteIsSet()
        {
            // Arrange
            var command = new DeleteProductCommand
            {
                Id = Guid.NewGuid(),
                Hard = true
            };

            var product = new Product(command.Id, "Product Name", 100, 90, "Description", ProductStatus.Available, 1, 10, new List<string> { "L", "M" }, Gender.Male, Guid.NewGuid());
            _mockUnitOfWork.Setup(uow => uow.ProductRepository.GetManageById(command.Id))
                .ReturnsAsync(product);

            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.DeleteSuccessfully);
            result.Data.Should().Be(null);
            product.IsDeleted.Should().BeFalse();

            _mockUnitOfWork.Verify(uow => uow.ProductRepository.Delete(It.IsAny<Product>()), Times.Once);
            _mockPublisher.Verify(p => p.Publish(It.IsAny<ModifiedProductEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFoundError_WhenDeleteFails()
        {
            // Arrange
            var command = new DeleteProductCommand
            {
                Id = Guid.NewGuid(),
                Hard = false
            };

            var product = new Product(Guid.NewGuid(), "Product Name", 100, 90, "Description", ProductStatus.Available, 1, 10, new List<string> { "L", "M" }, Gender.Male, Guid.NewGuid());
            product.AddImage("imageUrl", 1);
            _mockUnitOfWork.Setup(uow => uow.ProductRepository.GetManageById(command.Id));

            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be(MessageCommon.NotFound);
        }
    }
}
