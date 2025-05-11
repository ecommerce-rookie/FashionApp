using Application.Features.FeedbackFeatures.Commands;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using FluentValidation.TestHelper;
using Infrastructure.Authentication.Services;
using Infrastructure.Authentication.Settings;
using Moq;

namespace Application.UnitTest.Application.Features.FeedbackTest.CreateFeedbackTest
{
    public class CreateFeedbackCommandValidatorTest
    {
        private readonly CreateFeedbackValidator _validator;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IAuthenticationService> _mockAuthService;

        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _productId = Guid.NewGuid();

        public CreateFeedbackCommandValidatorTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockAuthService = new Mock<IAuthenticationService>();

            var userMock = new UserAuthenModel
            {
                Role = UserRole.Customer,
                UserId = _userId
            };
            _mockAuthService.Setup(x => x.User).Returns(userMock);

            _mockUnitOfWork.Setup(x => x.FeedbackRepository.CheckExistUserInProduct(_userId, _productId))
                .ReturnsAsync(false); // simulate that user hasn't given feedback yet

            _validator = new CreateFeedbackValidator(_mockUnitOfWork.Object, _mockAuthService.Object);
        }

        [Fact]
        public async Task Should_Have_Error_When_Rating_Is_Null()
        {
            var command = new CreateFeedbackCommand { Rating = null };

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Rating)
                .WithErrorMessage("Rating is required");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public async Task Should_Have_Error_When_Rating_Is_Out_Of_Range(int rating)
        {
            var command = new CreateFeedbackCommand { Rating = rating };

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Rating)
                .WithErrorMessage("Rating must be between 1 and 5");
        }

        [Fact]
        public async Task Should_Have_Error_When_Content_Is_Null()
        {
            var command = new CreateFeedbackCommand { Content = null };

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Content)
                .WithErrorMessage("Content is required");
        }

        [Fact]
        public async Task Should_Have_Error_When_Content_Is_Too_Short()
        {
            var command = new CreateFeedbackCommand { Content = "a" };

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Content)
                .WithErrorMessage("Minimum length for content is 2 characters");
        }

        [Fact]
        public async Task Should_Have_Error_When_Content_Is_Too_Long()
        {
            var longContent = new string('a', 3001);
            var command = new CreateFeedbackCommand { Content = longContent };

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Content)
                .WithErrorMessage("Content cannot exceed 3000 characters");
        }

        [Fact]
        public async Task Should_Have_Error_When_ProductId_Is_Default()
        {
            var command = new CreateFeedbackCommand
            {
                ProductId = Guid.Empty,
                Rating = 5,
                Content = "Nice product"
            };

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.ProductId)
                .WithErrorMessage("ProductId is required");
        }

        [Fact]
        public async Task Should_Have_Error_When_User_Already_Feedbacked()
        {
            _mockUnitOfWork.Setup(x => x.FeedbackRepository.CheckExistUserInProduct(_userId, _productId))
                .ReturnsAsync(true); // simulate user already gave feedback

            var command = new CreateFeedbackCommand
            {
                ProductId = _productId,
                Rating = 4,
                Content = "Good one"
            };

            var result = await _validator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.ProductId)
                .WithErrorMessage("You have already given feedback for this product");
        }

        [Fact]
        public async Task Should_Pass_When_Valid()
        {
            var command = new CreateFeedbackCommand
            {
                ProductId = _productId,
                Rating = 4,
                Content = "Good product!"
            };

            var result = await _validator.ValidateAsync(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
