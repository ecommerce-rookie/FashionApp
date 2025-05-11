using Application.Features.UserFeatures.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.UnitTest.Application.Features.UserTest.CreateUserTest
{
    public class CreateUserCommandValidatorTests
    {
        private readonly CreateUserCommandValidator _validator;

        public CreateUserCommandValidatorTests()
        {
            _validator = new CreateUserCommandValidator();
        }

        [Fact]
        public void Validate_Should_Fail_When_LastName_Is_Empty()
        {
            var command = new CreateUserCommand { LastName = "" };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "LastName");
        }

        [Fact]
        public void Validate_Should_Fail_When_Avatar_Too_Large()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(3 * 1024 * 1024); // 3MB

            var command = new CreateUserCommand
            {
                LastName = "Doe",
                Avatar = fileMock.Object
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Avatar");
        }

        [Fact]
        public void Validate_Should_Pass_With_Valid_Input()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(1 * 1024 * 1024); // 1MB

            var command = new CreateUserCommand
            {
                LastName = "Doe",
                Avatar = fileMock.Object
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}
