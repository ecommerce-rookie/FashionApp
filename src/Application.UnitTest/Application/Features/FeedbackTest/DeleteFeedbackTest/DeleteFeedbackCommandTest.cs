using Application.Features.FeedbackFeatures.Commands;
using Application.Messages;
using Domain.Aggregates.FeedbackAggregate.Entities;
using Domain.Aggregates.FeedbackAggregate.Events;
using Domain.Repositories;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using MediatR;
using Moq;
using System.Net;

namespace Application.UnitTest.Application.Features.FeedbackTest.DeleteFeedbackTest
{
    public class DeleteFeedbackCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IFeedbackRepository> _mockFeedbackRepository;
        private readonly Mock<IPublisher> _mockPublisher;
        private readonly DeleteFeedbackCommandHandler _handler;

        public DeleteFeedbackCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockFeedbackRepository = new Mock<IFeedbackRepository>();
            _mockPublisher = new Mock<IPublisher>();

            _mockUnitOfWork.Setup(u => u.FeedbackRepository).Returns(_mockFeedbackRepository.Object);
            _handler = new DeleteFeedbackCommandHandler(_mockUnitOfWork.Object, _mockPublisher.Object);
        }

        [Fact]
        public async Task Should_Return_NotFound_When_Feedback_Does_Not_Exist()
        {
            // Arrange
            var command = new DeleteFeedbackCommand { Id = Guid.NewGuid() };
            _mockFeedbackRepository.Setup(r => r.GetById(command.Id)).ReturnsAsync((Feedback?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be(MessageCommon.NotFound);
            result.Data.Should().Be(command.Id);
        }

        [Fact]
        public async Task Should_Delete_Hard_When_Hard_Delete_Is_True_And_Succeed()
        {
            // Arrange
            var feedback = new Feedback();
            var command = new DeleteFeedbackCommand { Id = Guid.NewGuid(), Hard = true };

            _mockFeedbackRepository.Setup(r => r.GetById(command.Id)).ReturnsAsync(feedback);
            _mockFeedbackRepository.Setup(r => r.Delete(feedback)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.DeleteSuccessfully);

            _mockFeedbackRepository.Verify(r => r.Delete(feedback), Times.Once);
            _mockPublisher.Verify(p => p.Publish(It.IsAny<ModifiedFeedbackEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_Delete_Soft_When_Hard_Delete_Is_False_And_Succeed()
        {
            // Arrange
            var mockFeedback = new Mock<Feedback>();
            var feedback = mockFeedback.Object;
            var command = new DeleteFeedbackCommand { Id = Guid.NewGuid(), Hard = false };

            _mockFeedbackRepository.Setup(r => r.GetById(command.Id)).ReturnsAsync(feedback);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.DeleteSuccessfully);

            _mockPublisher.Verify(p => p.Publish(It.IsAny<ModifiedFeedbackEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Should_Return_InternalServerError_When_SaveChanges_Fails()
        {
            // Arrange
            var mockFeedback = new Mock<Feedback>();
            var feedback = mockFeedback.Object;
            var command = new DeleteFeedbackCommand { Id = Guid.NewGuid() };

            _mockFeedbackRepository.Setup(r => r.GetById(command.Id)).ReturnsAsync(feedback);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.InternalServerError);
            result.Message.Should().Be(MessageCommon.DeleteFailed);
        }
    }
}
