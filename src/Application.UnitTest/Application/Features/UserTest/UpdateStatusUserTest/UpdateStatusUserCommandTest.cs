using Application.Features.UserFeatures.Commands;
using Application.Messages;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Exceptions;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Infrastructure.HttpClients;
using Moq;
using System.Net;

namespace Application.UnitTest.Application.Features.UserTest.UpdateStatusUserTest
{
    public class UpdateStatusUserCommandHandlerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IHttpService> _mockHttpService;
        private readonly UpdateStatusUserCommandHandler _handler;

        public UpdateStatusUserCommandHandlerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockHttpService = new Mock<IHttpService>();
            _handler = new UpdateStatusUserCommandHandler(_mockUnitOfWork.Object, _mockHttpService.Object);
        }

        [Fact]
        public async Task Handle_Should_Throw_ValidationException_When_User_NotFound()
        {
            // Arrange
            var command = new UpdateStatusUserCommand
            {
                UserId = Guid.NewGuid(),
                Status = UserStatus.Active
            };

            _mockUnitOfWork.Setup(u => u.UserRepository.GetById(command.UserId.Value)).ReturnsAsync((User)null!);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("One or more validation failures have occurred.");
        }

        [Fact]
        public async Task Handle_Should_Return_InternalServerError_When_SaveChanges_Fails()
        {
            // Arrange
            var command = new UpdateStatusUserCommand
            {
                UserId = Guid.NewGuid(),
                Status = UserStatus.Active
            };

            var userMock = new Mock<User>(command.UserId.Value, "test@email.com", "First", "Last", "123456", "https://avatar.iran.liara.run/username?username=test", UserStatus.Active, UserRole.Customer);
            userMock.Object.UpdateStatus(command.Status);

            _mockUnitOfWork.Setup(u => u.UserRepository.GetById(command.UserId.Value)).ReturnsAsync(userMock.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(MessageCommon.UpdateFailed);
        }

        [Fact]
        public async Task Handle_Should_Return_InternalServerError_And_Rollback_When_HttpService_Fails()
        {
            // Arrange
            var command = new UpdateStatusUserCommand
            {
                UserId = Guid.NewGuid(),
                Status = UserStatus.Active
            };

            var userMock = new Mock<User>(command.UserId.Value, "test@email.com", "First", "Last", "123456", "https://avatar.iran.liara.run/username?username=test", UserStatus.Active, UserRole.Customer);
            userMock.Object.UpdateStatus(command.Status);

            _mockUnitOfWork.Setup(u => u.UserRepository.GetById(command.UserId.Value)).ReturnsAsync(userMock.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockHttpService.Setup(h => h.UpdateStatusUser(command.UserId.Value, command.Status.ToString())).ReturnsAsync("Service Error");
            _mockUnitOfWork.Setup(u => u.RollbackTransaction(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be("Service Error");
            _mockUnitOfWork.Verify(u => u.RollbackTransaction(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_OK_When_All_Success()
        {
            // Arrange
            var command = new UpdateStatusUserCommand
            {
                UserId = Guid.NewGuid(),
                Status = UserStatus.Active
            };

            var userMock = new Mock<User>(command.UserId.Value, "test@email.com", "First", "Last", "123456", "https://avatar.iran.liara.run/username?username=test", UserStatus.Active, UserRole.Customer);
            userMock.Object.UpdateStatus(command.Status);

            _mockUnitOfWork.Setup(u => u.UserRepository.GetById(command.UserId.Value)).ReturnsAsync(userMock.Object);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mockHttpService.Setup(h => h.UpdateStatusUser(command.UserId.Value, command.Status.ToString())).ReturnsAsync((string?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.UpdateSuccesfully);
        }
    }
}
