using Application.Features.FeedbackFeatures.Enums;
using Application.Features.FeedbackFeatures.Models;
using Application.Features.FeedbackFeatures.Queries;
using Application.Features.UserFeatures.Models;
using Domain.Aggregates.FeedbackAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Models.Common;
using Domain.Repositories;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace Application.UnitTest.Application.Features.FeedbackTest.GetFeedbackTest
{
    public class GetListFeedbackQueryHandlerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IFeedbackRepository> _mockFeedbackRepository;
        private readonly GetListFeedbackQueryHandler _handler;

        public GetListFeedbackQueryHandlerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockFeedbackRepository = new Mock<IFeedbackRepository>();

            _mockUnitOfWork.Setup(u => u.FeedbackRepository).Returns(_mockFeedbackRepository.Object);

            _handler = new GetListFeedbackQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_PagedList_Of_FeedbackResponseModel()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request = new GetListFeedbackQuery
            {
                ProductId = productId,
                Page = 1,
                EachPage = 10
            };

            var feedbackList = new List<Feedback>
            {
                new Feedback
                {
                    ProductId = productId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Content = "Very good",
                    Rating = 5,
                    CreatedByNavigation = new User(Guid.NewGuid(), "test@gmail.com", "first name", "last name", "093128361", "http://image.com/avatar.png", UserStatus.Active, UserRole.Customer)
                }
            };

            var expectedResponse = new PagedList<FeedbackResponseModel>(
                new List<FeedbackResponseModel>
                {
                new FeedbackResponseModel
                {
                    CreatedAt = feedbackList[0].CreatedAt,
                    UpdatedAt = feedbackList[0].UpdatedAt,
                    Content = feedbackList[0].Content,
                    Rating = feedbackList[0].Rating,
                    Author = new AuthorResponseModel
                    {
                        Avatar = feedbackList[0].CreatedByNavigation.Avatar.Url,
                        FirstName = feedbackList[0].CreatedByNavigation.FirstName,
                        LastName = feedbackList[0].CreatedByNavigation.LastName,
                        Id = feedbackList[0].CreatedByNavigation.Id
                    }
                }
                },
                1, 10, 1
            );

            _mockFeedbackRepository.Setup(repo =>
                repo.GetAll(
                    It.IsAny<Expression<Func<Feedback, bool>>>(),
                    It.IsAny<Expression<Func<Feedback, FeedbackResponseModel>>>(),
                    request.Page,
                    request.EachPage,
                    FeedbackSortBy.CreatedAt.ToString(),
                    true))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);

            _mockFeedbackRepository.Verify(repo =>
                repo.GetAll(
                    It.Is<Expression<Func<Feedback, bool>>>(expr =>
                        expr.Compile().Invoke(new Feedback { ProductId = productId })),
                    It.IsAny<Expression<Func<Feedback, FeedbackResponseModel>>>(),
                    request.Page,
                    request.EachPage,
                    FeedbackSortBy.CreatedAt.ToString(),
                    true),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_EmptyList_When_NoFeedbacksFound()
        {
            // Arrange
            var request = new GetListFeedbackQuery
            {
                ProductId = Guid.NewGuid(),
                Page = 1,
                EachPage = 10
            };

            var emptyResult = new PagedList<FeedbackResponseModel>(new List<FeedbackResponseModel>(), 1, 10, 0);

            _mockFeedbackRepository.Setup(repo =>
                repo.GetAll(It.IsAny<Expression<Func<Feedback, bool>>>(),
                            It.IsAny<Expression<Func<Feedback, FeedbackResponseModel>>>(),
                            request.Page, request.EachPage,
                            FeedbackSortBy.CreatedAt.ToString(), true))
                .ReturnsAsync(emptyResult);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().BeEmpty();
            result.Count().Should().Be(0);
        }

    }
}
