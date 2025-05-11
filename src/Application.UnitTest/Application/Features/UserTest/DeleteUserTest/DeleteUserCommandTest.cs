using Application.Features.UserFeatures.Commands;
using Application.Messages;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Moq;
using System.Net;

namespace Application.UnitTest.Application.Features.UserTest.DeleteUserTest
{
    public class DeleteUserCommandHandlerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly DeleteUserCommandHandler _handler;

        public DeleteUserCommandHandlerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _handler = new DeleteUserCommandHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_NotFound_When_User_Is_Null()
        {
            // Arrange
            var command = new DeleteUserCommand { Id = Guid.NewGuid() };

            _mockUnitOfWork.Setup(u => u.UserRepository.GetById(command.Id))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be(MessageCommon.NotFound);
            result.Data.Should().Be(command.Id);
        }

        [Fact]
        public async Task Handle_Should_Call_Hard_Delete_And_Return_Ok_When_Successful()
        {
            // Arrange
            var command = new DeleteUserCommand { Id = Guid.NewGuid(), Hard = true };
            var user = new Mock<User>(Guid.NewGuid(), "test@email.com", "First", "Last", "123456", "https://avatar.iran.liara.run/username?username=test", UserStatus.Active, UserRole.Customer);

            _mockUnitOfWork.Setup(u => u.UserRepository.GetById(command.Id)).ReturnsAsync(user.Object);
            _mockUnitOfWork.Setup(u => u.UserRepository.Delete(user.Object)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.DeleteSuccessfully);
        }

        [Fact]
        public async Task Handle_Should_Call_Soft_Delete_And_Return_Ok_When_Successful()
        {
            // Arrange
            var command = new DeleteUserCommand { Id = Guid.NewGuid(), Hard = false };
            var userMock = new Mock<User>(Guid.NewGuid(), "test@email.com", "First", "Last", "123456", "https://avatar.iran.liara.run/username?username=test", UserStatus.Active, UserRole.Customer);

            _mockUnitOfWork.Setup(u => u.UserRepository.GetById(command.Id)).ReturnsAsync(userMock.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.DeleteSuccessfully);
        }

        [Fact]
        public async Task Handle_Should_Return_InternalServerError_When_Save_Fails()
        {
            // Arrange
            var command = new DeleteUserCommand { Id = Guid.NewGuid() };
            var userMock = new Mock<User>(Guid.NewGuid(), "test@email.com", "First", "Last", "123456", "https://avatar.iran.liara.run/username?username=test", UserStatus.Active, UserRole.Customer);

            _mockUnitOfWork.Setup(u => u.UserRepository.GetById(command.Id)).ReturnsAsync(userMock.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(MessageCommon.DeleteFailed);
        }
    }
}
