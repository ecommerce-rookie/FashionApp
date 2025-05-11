using Application.Features.UserFeatures.Models;
using Application.Features.UserFeatures.Queries;
using AutoMapper;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using Infrastructure.Authentication.Services;
using Infrastructure.Authentication.Settings;
using Moq;

namespace Application.UnitTest.Application.Features.UserTest.GetUserTest
{
    public class GetListUserQueryHandlerTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetListUserQueryHandler _handler;

        public GetListUserQueryHandlerTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetListUserQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object, _mockAuthService.Object);
        }

        private static List<User> GenerateUsers(int count)
        {
            var users = new List<User>();
            for (int i = 0; i < count; i++)
            {
                users.Add(new User(
                    Guid.NewGuid(),
                    $"email{i}@gmail.com",
                    $"User{i}",
                    $"Last{i}",
                    $"0{i + 1}123213",
                    $"http://image.com/avatar{i}.png",
                    UserStatus.Active,
                    i % 2 == 0 ? UserRole.Customer : UserRole.Staff
                ));
            }

            return users;
        }

        [Fact]
        public async Task Handle_Should_Return_Users_For_Staff_Without_Admin()
        {
            // Arrange
            var userAuth = new UserAuthenModel { Role = UserRole.Staff };
            _mockAuthService.Setup(x => x.User).Returns(userAuth);

            var roles = new List<UserRole> { UserRole.Customer, UserRole.Admin };
            var expectedRoles = new List<UserRole> { UserRole.Customer };

            var users = new PagedList<User>(GenerateUsers(10));

            _mockUnitOfWork.Setup(x => x.UserRepository.GetUsers(
                        1, 10, It.IsAny<IEnumerable<UserRole>>(), null, null, UserRole.Staff))
                        .ReturnsAsync(users);


            _mockMapper.Setup(m => m.Map<PagedList<UserPreviewResponseModel>>(users))
                .Returns(new PagedList<UserPreviewResponseModel>());

            var query = new GetListUserQuery
            {
                Page = 1,
                EachPage = 10,
                Roles = roles
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _mockUnitOfWork.Verify(x => x.UserRepository.GetUsers(1, 10, It.IsAny<IEnumerable<UserRole>>(), null, null, UserRole.Staff), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Users_For_Admin()
        {
            // Arrange
            var userAuth = new UserAuthenModel { Role = UserRole.Admin };
            _mockAuthService.Setup(x => x.User).Returns(userAuth);

            var users = new PagedList<User>(GenerateUsers(3));

            _mockUnitOfWork.Setup(x => x.UserRepository.GetUsers(
                1, 10, null, null, null, UserRole.Admin))
                .ReturnsAsync(users);

            _mockMapper.Setup(m => m.Map<PagedList<UserPreviewResponseModel>>(users))
                .Returns(new PagedList<UserPreviewResponseModel>());

            var query = new GetListUserQuery
            {
                Page = 1,
                EachPage = 10
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _mockUnitOfWork.Verify(x => x.UserRepository.GetUsers(1, 10, null, null, null, UserRole.Admin), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Users_For_Staff_When_Roles_Is_Null()
        {
            // Arrange
            var userAuth = new UserAuthenModel { Role = UserRole.Staff };
            _mockAuthService.Setup(x => x.User).Returns(userAuth);

            var users = new PagedList<User>(GenerateUsers(1));

            _mockUnitOfWork.Setup(x => x.UserRepository.GetUsers(
                1, 10, null, null, null, UserRole.Staff)).ReturnsAsync(users);

            _mockMapper.Setup(m => m.Map<PagedList<UserPreviewResponseModel>>(users))
                .Returns(new PagedList<UserPreviewResponseModel>());

            var query = new GetListUserQuery
            {
                Page = 1,
                EachPage = 10,
                Roles = null
            };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _mockUnitOfWork.Verify(x => x.UserRepository.GetUsers(1, 10, null, null, null, UserRole.Staff), Times.Once);
        }

    }
}
