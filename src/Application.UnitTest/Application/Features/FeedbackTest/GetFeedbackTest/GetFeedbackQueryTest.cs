using Application.Features.FeedbackFeatures.Models;
using Application.Features.FeedbackFeatures.Queries;
using Application.Messages;
using AutoMapper;
using Domain.Aggregates.FeedbackAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Infrastructure.Authentication.Services;
using Infrastructure.Authentication.Settings;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitTest.Application.Features.FeedbackTest.GetFeedbackTest
{
    public class GetFeedbackQueryHandlerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetFeedbackQueryHandler _handler;
        private readonly Guid _userId;

        public GetFeedbackQueryHandlerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockMapper = new Mock<IMapper>();
            _userId = Guid.NewGuid();

            var userMock = new UserAuthenModel
            {
                Role = UserRole.Customer,
                UserId = _userId
            };
            _mockAuthService.Setup(x => x.User).Returns(userMock);

            _handler = new GetFeedbackQueryHandler(
                _mockUnitOfWork.Object,
                _mockAuthService.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task Handle_Should_Return_NotFound_When_Feedback_Is_Null()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var query = new GetFeedbackQuery { ProductId = productId };

            _mockUnitOfWork.Setup(u => u.FeedbackRepository.GetMyFeedback(_userId, productId))
                .ReturnsAsync((Feedback?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be(MessageCommon.NotFound);
            result.Data.Should().BeNull();

            _mockUnitOfWork.Verify(u => u.FeedbackRepository.GetMyFeedback(_userId, productId), Times.Once);
            _mockMapper.Verify(m => m.Map<FeedbackResponseModel>(It.IsAny<Feedback>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Return_OK_With_Data_When_Feedback_Exists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var query = new GetFeedbackQuery { ProductId = productId };

            var feedback = new Feedback
            {
                Id = Guid.NewGuid(),
                Content = "Excellent!",
                ProductId = productId,
                UserId = _userId
            };

            var mappedResponse = new FeedbackResponseModel
            {
                Content = feedback.Content
            };

            _mockUnitOfWork.Setup(u => u.FeedbackRepository.GetMyFeedback(_userId, productId))
                .ReturnsAsync(feedback);

            _mockMapper.Setup(m => m.Map<FeedbackResponseModel>(feedback))
                .Returns(mappedResponse);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.GetSuccesfully);
            result.Data.Should().BeEquivalentTo(mappedResponse);

            _mockUnitOfWork.Verify(u => u.FeedbackRepository.GetMyFeedback(_userId, productId), Times.Once);
            _mockMapper.Verify(m => m.Map<FeedbackResponseModel>(feedback), Times.Once);
        }
    }
}
