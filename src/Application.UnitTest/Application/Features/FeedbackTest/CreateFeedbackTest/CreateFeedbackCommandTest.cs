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

namespace Application.UnitTest.Application.Features.FeedbackTest.CreateFeedbackTest
{
    public class CreateFeedbackCommandTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly CreateFeedbackCommandHandler _handler;

        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _productId = Guid.NewGuid();

        public CreateFeedbackCommandTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockPublisher = new Mock<IPublisher>();

            var userMock = new UserAuthenModel
            {
                Role = UserRole.Customer,
                UserId = _userId
            };
            _mockAuthService.Setup(x => x.User).Returns(userMock);

            _handler = new CreateFeedbackCommandHandler(_mockUnitOfWork.Object, _mockAuthService.Object, _mockPublisher.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Created_When_Successful()
        {
            // Arrange
            var command = new CreateFeedbackCommand
            {
                ProductId = _productId,
                Rating = 5,
                Content = "Great product!"
            };

            _mockUnitOfWork.Setup(x => x.FeedbackRepository.Add(It.IsAny<Feedback>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Status.Should().Be(HttpStatusCode.Created);
            response.Message.Should().Be(MessageCommon.CreateSuccesfully);

            _mockUnitOfWork.Verify(x => x.FeedbackRepository.Add(It.IsAny<Feedback>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockPublisher.Verify(x => x.Publish(It.IsAny<ModifiedFeedbackEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_InternalServerError_When_Save_Fails()
        {
            // Arrange
            var command = new CreateFeedbackCommand
            {
                ProductId = _productId,
                Rating = 3,
                Content = "Decent"
            };

            _mockUnitOfWork.Setup(x => x.FeedbackRepository.Add(It.IsAny<Feedback>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Status.Should().Be(HttpStatusCode.InternalServerError);
            response.Message.Should().Be(MessageCommon.CreateFailed);

            _mockPublisher.Verify(x => x.Publish(It.IsAny<ModifiedFeedbackEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_When_Rating_Or_Content_Is_Null()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var mockAuthService = new Mock<IAuthenticationService>();
            mockAuthService.Setup(x => x.User).Returns(new UserAuthenModel
            {
                UserId = userId,
                Role = UserRole.Customer
            });

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockPublisher = new Mock<IPublisher>();

            var handler = new CreateFeedbackCommandHandler(mockUnitOfWork.Object, mockAuthService.Object, mockPublisher.Object);

            var command = new CreateFeedbackCommand
            {
                ProductId = productId,
                Rating = null,
                Content = null
            };

            // Act
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*nullable object must have a value*");
        }

    }
}
