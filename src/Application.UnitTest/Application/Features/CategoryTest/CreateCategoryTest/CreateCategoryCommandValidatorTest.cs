using Application.Features.CategoryFeatures.Commands;
using FluentAssertions;
using FluentValidation.Results;

namespace Application.UnitTest.Application.Features.CategoryTest.CreateCategoryTest
{
    public class CreateCategoryCommandValidatorTest
    {
        private readonly CreateCategoryCommandValidator _validator;

        public CreateCategoryCommandValidatorTest()
        {
            _validator = new CreateCategoryCommandValidator();
        }

        [Fact]
        public void Should_Fail_When_Name_Is_Empty()
        {
            // Arrange
            var command = new CreateCategoryCommand { Name = string.Empty };

            // Act
            ValidationResult result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == nameof(CreateCategoryCommand.Name) &&
                e.ErrorMessage == "Name is required");
        }

        [Fact]
        public void Should_Fail_When_Name_Exceeds_Max_Length()
        {
            // Arrange
            var command = new CreateCategoryCommand { Name = new string('A', 101) };

            // Act
            ValidationResult result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e =>
                e.PropertyName == nameof(CreateCategoryCommand.Name) &&
                e.ErrorMessage == "Name must not exceed 100 characters");
        }

        [Fact]
        public void Should_Succeed_When_Name_Is_Valid()
        {
            // Arrange
            var command = new CreateCategoryCommand { Name = "Electronics" };

            // Act
            ValidationResult result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
}
