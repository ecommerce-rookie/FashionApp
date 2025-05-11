using Algolia.Search.Models.Search;
using Application.Features.UserFeatures.Models;
using Application.Features.UserFeatures.Queries;
using Application.Messages;
using AutoMapper;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Infrastructure.Authentication.Services;
using Infrastructure.Authentication.Settings;
using Moq;
using System.Net;

namespace Application.UnitTest.Application.Features.UserTest.GetUserTest
{
    public class GetAuthorQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IAuthenticationService> _authServiceMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly GetAuthorQueryHandler _handler;

        public GetAuthorQueryHandlerTests()
        {
            _handler = new GetAuthorQueryHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _authServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userMock = new UserAuthenModel
            {
                Role = UserRole.Customer,
                UserId = userId
            };
            _authServiceMock.Setup(x => x.User).Returns(userMock);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetById(userId))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _handler.Handle(new GetAuthorQuery(), CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.NotFound);
            result.Message.Should().Be(MessageCommon.NotFound);
            result.Data.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldReturnAuthor_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User(userId, "test@gmail.com", "John", "Doe", "093128361", "http://image.com/avatar.png", UserStatus.Active, UserRole.Customer);
            var expectedModel = new AuthorResponseModel { Id = userId, FirstName = "John", LastName = "Doe" };

            var userMock = new UserAuthenModel
            {
                Role = UserRole.Customer,
                UserId = userId
            };
            _authServiceMock.Setup(x => x.User).Returns(userMock);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetById(userId)).ReturnsAsync(user);
            _mapperMock.Setup(x => x.Map<AuthorResponseModel>(user)).Returns(expectedModel);

            // Act
            var result = await _handler.Handle(new GetAuthorQuery(), CancellationToken.None);

            // Assert
            result.Status.Should().Be(HttpStatusCode.OK);
            result.Message.Should().Be(MessageCommon.GetSuccesfully);
            result.Data.Should().BeEquivalentTo(expectedModel);
        }
    }
}
