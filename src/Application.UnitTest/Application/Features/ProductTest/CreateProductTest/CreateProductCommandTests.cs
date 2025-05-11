using Application.Features.ProductFeatures.Commands;
using Application.Messages;
using CloudinaryDotNet.Actions;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Infrastructure.Authentication.Services;
using Infrastructure.Authentication.Settings;
using Infrastructure.Storage;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Net;
using static Infrastructure.Storage.Cloudinary.Internals.CloudinaryOptions;

namespace Application.UnitTest.Application.Features.ProductTest.CreateProductTest
{
    public class CreateProductCommandHandlerTests
    {
        private readonly CreateProductCommandHandler _handler;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAuthenticationService> _mockAuthenticationService;
        private readonly Mock<IStorageService> _mockStorageService;
        private readonly Mock<IPublisher> _mockPublisher;

        public CreateProductCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthenticationService = new Mock<IAuthenticationService>();
            _mockStorageService = new Mock<IStorageService>();
            _mockPublisher = new Mock<IPublisher>();

            _handler = new CreateProductCommandHandler(
                _mockUnitOfWork.Object,
                _mockAuthenticationService.Object,
                _mockStorageService.Object,
                _mockPublisher.Object
            );
        }

        [Fact]
        public async Task Should_Return_Forbidden_When_User_Is_Not_Staff()
        {
            // Arrange
            var userMock = new UserAuthenModel
            {
                Role = UserRole.Admin,
                UserId = Guid.NewGuid()
            };

            // Setup the mock user
            _mockAuthenticationService.Setup(x => x.User).Returns(userMock);

            var command = new CreateProductCommand
            {
                Name = "Product",
                UnitPrice = 100,
                PurchasePrice = 80,
                CategoryId = 1,
                Quantity = 10,
                Status = ProductStatus.Available,
                Gender = Gender.Male
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.Forbidden);
            result.Message.Should().Be(MessageCommon.Forbidden);
        }

        [Fact]
        public async Task Should_Create_Product_Successfully_When_Data_Is_Valid()
        {
            // Arrange
            var userMock = new UserAuthenModel
            {
                Role = UserRole.Staff,
                UserId = Guid.NewGuid()
            };

            // Setup the mock user
            _mockAuthenticationService.Setup(x => x.User).Returns(userMock);

            // Setup the mock unit of work and storage service
            _mockUnitOfWork.Setup(x => x.CategoryRepository.CheckCategoryExist(It.IsAny<int>())).ReturnsAsync(true);
            _mockUnitOfWork.Setup(x => x.ProductRepository.Add(It.IsAny<Product>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockStorageService.Setup(x => x.UploadImage(It.IsAny<IFormFile>(), It.IsAny<ImageFolder>(), It.IsAny<ImageFormat>(), It.IsAny<string>()))
                .ReturnsAsync(Mock.Of<UploadResult>(u => u.Url == new Uri("http://example.com/image.png")));

            // Create a valid command
            var command = new CreateProductCommand
            {
                Name = "Valid Product",
                UnitPrice = 100,
                PurchasePrice = 80,
                CategoryId = 1,
                Quantity = 10,
                Status = ProductStatus.Available,
                Gender = Gender.Male,
                Files = new List<IFormFile> { new Mock<IFormFile>().Object }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.Created);
            result.Message.Should().Be(MessageCommon.CreateSuccesfully);
        }


        [Fact]
        public async Task Should_Return_InternalServerError_When_Save_Fails()
        {
            // Arrange
            var userMock = new UserAuthenModel
            {
                Role = UserRole.Staff,
                UserId = Guid.NewGuid()
            };

            // Setup the mock user
            _mockAuthenticationService.Setup(x => x.User).Returns(userMock); ;
            _mockUnitOfWork.Setup(x => x.CategoryRepository.CheckCategoryExist(It.IsAny<int>())).ReturnsAsync(true);
            _mockUnitOfWork.Setup(x => x.ProductRepository.Add(It.IsAny<Product>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            var command = new CreateProductCommand
            {
                Name = "Valid Product",
                UnitPrice = 100,
                PurchasePrice = 80,
                CategoryId = 1,
                Quantity = 10,
                Status = ProductStatus.Available,
                Gender = Gender.Male
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(MessageCommon.CreateFailed);
        }
    }

}
