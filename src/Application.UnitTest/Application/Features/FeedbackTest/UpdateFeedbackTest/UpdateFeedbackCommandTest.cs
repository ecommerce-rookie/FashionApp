using Application.Features.FeedbackFeatures.Commands;
using Application.Messages;
using Domain.Aggregates.FeedbackAggregate.Entities;
using Domain.Aggregates.FeedbackAggregate.Events;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Infrastructure.Authentication.Services;
using Infrastructure.Authentication.Settings;
using MediatR;
using Moq;
using System.Net;

namespace Application.UnitTest.Application.Features.FeedbackTest.UpdateFeedbackTest
{
    public class UpdateFeedbackCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly UpdateFeedbackCommandHandler _handler;

        public UpdateFeedbackCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockPublisher = new Mock<IPublisher>();

            var userMock = new UserAuthenModel
            {
                Role = UserRole.Customer,
                UserId = Guid.NewGuid()
            };
            _mockAuthService.Setup(x => x.User).Returns(userMock);

            _handler = new UpdateFeedbackCommandHandler(_mockUnitOfWork.Object, _mockPublisher.Object, _mockAuthService.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_NotFound_When_Feedback_Is_Null()
        {
            // Arrange
            var command = new UpdateFeedbackCommand
            {
                ProductId = Guid.NewGuid(),
                Rating = 4,
                Content = "Updated content"
            };

            _mockUnitOfWork.Setup(x => x.FeedbackRepository.GetMyFeedback(It.IsAny<Guid>(), command.ProductId))
                .ReturnsAsync((Feedback)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be(MessageCommon.NotFound);
            result.Data.Should().Be(command.ProductId);
        }

        [Fact]
        public async Task Handle_Should_Update_Feedback_And_Return_Success_When_Save_Succeeds()
        {
            // Arrange
            var command = new UpdateFeedbackCommand
            {
                ProductId = Guid.NewGuid(),
                Rating = 5,
                Content = "Great!"
            };

            var userId = _mockAuthService.Object.User.UserId;

            var feedback = new Feedback(Guid.NewGuid(), "Old Content", userId, command.ProductId, 3);

            _mockUnitOfWork.Setup(x => x.FeedbackRepository.GetMyFeedback(userId, command.ProductId))
                .ReturnsAsync(feedback);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.UpdateSuccesfully);
            result.Data.Should().Be(feedback.Id);
            feedback.Content.Should().Be(command.Content);
            feedback.Rating.Should().Be(command.Rating);

            _mockPublisher.Verify(p => p.Publish(It.IsAny<ModifiedFeedbackEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_InternalServerError_When_Save_Fails()
        {
            // Arrange
            var command = new UpdateFeedbackCommand
            {
                ProductId = Guid.NewGuid(),
                Rating = 2,
                Content = "Average"
            };

            var userId = _mockAuthService.Object.User.UserId;

            var feedback = new Feedback(Guid.NewGuid(), "Previous", userId, command.ProductId, 1);

            _mockUnitOfWork.Setup(x => x.FeedbackRepository.GetMyFeedback(userId, command.ProductId))
                .ReturnsAsync(feedback);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(MessageCommon.UpdateFailed);
            result.Data.Should().Be(command.ProductId);
        }
    }
}
