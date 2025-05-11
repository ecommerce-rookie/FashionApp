using Application.Features.UserFeatures.Commands;
using Application.Messages;
using CloudinaryDotNet.Actions;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Infrastructure.Authentication.Services;
using Infrastructure.Authentication.Settings;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Net;
using static Infrastructure.Storage.Cloudinary.Internals.CloudinaryOptions;

namespace Application.UnitTest.Application.Features.UserTest.CreateUserTest
{
    public class CreateUserCommandHandlerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IStorageService> _mockStorageService;
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockStorageService = new Mock<IStorageService>();
            _mockAuthService = new Mock<IAuthenticationService>();

            _mockAuthService.Setup(x => x.User).Returns(new UserAuthenModel
            {
                Email = "test@example.com",
                Role = UserRole.Customer,
                UserId = Guid.NewGuid()
            });

            _handler = new CreateUserCommandHandler(
                _mockUnitOfWork.Object,
                _mockStorageService.Object,
                _mockAuthService.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Created_When_Successful_Without_Avatar()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                FirstName = "John",
                LastName = "Doe",
                Phone = "1234567890",
                Address = "123 Main St",
                Avatar = null
            };

            _mockUnitOfWork.Setup(u => u.UserRepository.Add(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.Created);
            result.Message.Should().Be(MessageCommon.CreateSuccesfully);
        }

        [Fact]
        public async Task Handle_Should_Return_Created_When_Successful_With_Avatar()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                FirstName = "Jane",
                LastName = "Smith",
                Phone = "0987654321",
                Avatar = new Mock<IFormFile>().Object
            };

            _mockStorageService.Setup(s => s.UploadImage(
                command.Avatar, ImageFolder.Avatar, ImageFormat.png, It.IsAny<string>()))
                .ReturnsAsync(new ImageUploadResult { Url = new Uri("http://image.com/avatar.png") });

            _mockUnitOfWork.Setup(u => u.UserRepository.Add(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.Created);
            result.Message.Should().Be(MessageCommon.CreateSuccesfully);
        }

        [Fact]
        public async Task Handle_Should_Return_InternalServerError_When_Save_Fails()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                FirstName = "Fail",
                LastName = "Case",
                Phone = "0000000000"
            };

            _mockUnitOfWork.Setup(u => u.UserRepository.Add(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(MessageCommon.CreateFailed);
        }
    }
}
