using Application.Features.ProductFeatures.Commands;
using Application.Features.ProductFeatures.Models;
using Application.Messages;
using CloudinaryDotNet.Actions;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Aggregates.ProductAggregate.Events;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Infrastructure.Storage;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Net;
using static Infrastructure.Storage.Cloudinary.Internals.CloudinaryOptions;

namespace Application.UnitTest.Application.Features.ProductTest.UpdateProductTest
{
    public class UpdateProductCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly Mock<IStorageService> _mockStorageService;
        private readonly UpdateProductCommandHandler _handler;

        public UpdateProductCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPublisher = new Mock<IPublisher>();
            _mockStorageService = new Mock<IStorageService>();

            _handler = new UpdateProductCommandHandler(
                _mockUnitOfWork.Object,
                _mockPublisher.Object,
                _mockStorageService.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Slug = "non-existing-slug"
            };

            _mockUnitOfWork.Setup(uow => uow.ProductRepository.GetDetail(It.IsAny<string>()))
                .ReturnsAsync((Product?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be(MessageCommon.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldUpdateProduct_WhenProductExists()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Slug = "existing-slug",
                Name = "Updated Product Name",
                UnitPrice = 120,
                PurchasePrice = 100,
                Status = ProductStatus.Available,
                CategoryId = 1,
                Quantity = 10,
                Files = new List<IFormFile>
                {
                    new Mock<IFormFile>().Object
                },
                Gender = Gender.Male,
                Description = "Updated description",
            };

            var existingProduct = new Product(Guid.NewGuid(), "Old Product", 100, 90, "Old description", ProductStatus.OutOfStock, 1, 5, new List<string> { "L", "M" }, Gender.Male, Guid.NewGuid());
            _mockUnitOfWork.Setup(uow => uow.ProductRepository.GetDetail(It.IsAny<string>())).ReturnsAsync(existingProduct);

            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockPublisher.Setup(p => p.Publish(It.IsAny<ModifiedProductEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            _mockStorageService.Setup(x => x.UploadImage(It.IsAny<IFormFile>(), It.IsAny<ImageFolder>(), It.IsAny<ImageFormat>(), It.IsAny<string>()))
                .ReturnsAsync(Mock.Of<UploadResult>(u => u.Url == new Uri("http://example.com/image.png")));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.UpdateSuccesfully);
            _mockUnitOfWork.Verify(uow => uow.ProductRepository.Update(It.IsAny<Product>()), Times.Once);
            _mockPublisher.Verify(p => p.Publish(It.IsAny<ModifiedProductEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnInternalServerError_WhenSaveFails()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Slug = "existing-slug"
            };

            var existingProduct = new Product(Guid.NewGuid(), "Old Product", 100, 90, "Old description", ProductStatus.OutOfStock, 1, 5, new List<string> { "L", "M" }, Gender.Male, Guid.NewGuid());
            _mockUnitOfWork.Setup(uow => uow.ProductRepository.GetDetail(It.IsAny<string>())).ReturnsAsync(existingProduct);

            // Mock the SaveChangesAsync method to return false
            _mockUnitOfWork.Setup(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(MessageCommon.UpdateFailed);
        }
    }
}
