using Application.Features.UserFeatures.Commands;
using Application.Messages;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Aggregates.UserAggregate.Events;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Infrastructure.Storage;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Net;
using static Infrastructure.Storage.Cloudinary.Internals.CloudinaryOptions;

namespace Application.UnitTest.Application.Features.UserTest.UpdateUserTest
{
    public class UpdateUserCommandHandlerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IStorageService> _mockStorageService;
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly UpdateUserCommandHandler _handler;

        public UpdateUserCommandHandlerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockStorageService = new Mock<IStorageService>();
            _mockPublisher = new Mock<IPublisher>();
            _handler = new UpdateUserCommandHandler(_mockUnitOfWork.Object, _mockStorageService.Object, _mockPublisher.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_NotFound_When_User_Does_Not_Exist()
        {
            // Arrange
            var command = new UpdateUserCommand { Id = Guid.NewGuid() };

            _mockUnitOfWork.Setup(u => u.UserRepository.GetById(command.Id))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be(MessageCommon.NotFound);
        }

        [Fact]
        public async Task Handle_Should_Return_InternalServerError_When_SaveChanges_Fails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserCommand
            {
                Id = userId,
                Email = "new@email.com"
            };

            var user = new Mock<User>(userId, "old@email.com", "First", "Last", "123456", "https://avatar.iran.liara.run/username?username=test", UserStatus.Active, UserRole.Customer);

            _mockUnitOfWork.Setup(u => u.UserRepository.GetById(userId)).ReturnsAsync(user.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(MessageCommon.UpdateFailed);
        }

        [Fact]
        public async Task Handle_Should_Update_User_Without_Avatar_And_Return_OK()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserCommand
            {
                Id = userId,
                Email = "new@email.com",
                FirstName = "NewFirst",
                LastName = "NewLast",
                Phone = "999888"
            };

            var avatar = "http://example.com/avatar.png";

            var user = new User(userId, "old@email.com", "First", "Last", "123456", avatar, UserStatus.Active, UserRole.Customer);

            _mockUnitOfWork.Setup(u => u.UserRepository.GetById(userId)).ReturnsAsync(user);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.UpdateSuccesfully);
            _mockStorageService.Verify(s => s.UploadImage(It.IsAny<IFormFile>(), It.IsAny<ImageFolder>(), It.IsAny<ImageFormat>(), It.IsAny<string>()), Times.Never);
            _mockPublisher.Verify(p => p.Publish(It.IsAny<ModifedUserEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task Handle_Should_Upload_Avatar_And_Return_OK()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var avatarFile = new Mock<IFormFile>();
            var command = new UpdateUserCommand
            {
                Id = userId,
                Email = "new@email.com",
                Avatar = avatarFile.Object
            };

            var oldAvatar = "http://old.com/avatar.png";
            var user = new User(userId, "old@email.com", "First", "Last", "123456", oldAvatar, UserStatus.Active, UserRole.Customer);

            _mockUnitOfWork.Setup(u => u.UserRepository.GetById(userId)).ReturnsAsync(user);
            _mockStorageService
                .Setup(s => s.UploadImage(It.IsAny<IFormFile>(), ImageFolder.Avatar, ImageFormat.png, string.Empty))
                .ReturnsAsync(Mock.Of<UploadResult>(u => u.Url == new Uri("http://example.com/image.png")));

            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.UpdateSuccesfully);
            _mockStorageService.Verify(s => s.UploadImage(command.Avatar, ImageFolder.Avatar, ImageFormat.png, string.Empty), Times.Once);
            _mockPublisher.Verify(p => p.Publish(It.IsAny<ModifedUserEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

    }

}
