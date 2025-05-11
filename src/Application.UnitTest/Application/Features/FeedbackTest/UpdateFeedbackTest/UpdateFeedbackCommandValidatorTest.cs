using Application.Features.FeedbackFeatures.Commands;
using FluentAssertions;

namespace Application.UnitTest.Application.Features.FeedbackTest.UpdateFeedbackTest
{
    public class UpdateFeedbackCommandValidatorTest
    {
        public class UpdateFeedbackValidatorTests
        {
            private readonly UpdateFeedbackValidator _validator;

            public UpdateFeedbackValidatorTests()
            {
                _validator = new UpdateFeedbackValidator();
            }

            [Fact]
            public void Should_Pass_When_All_Fields_Valid()
            {
                var command = new UpdateFeedbackCommand
                {
                    Rating = 4,
                    Content = "Very good product!"
                };

                var result = _validator.Validate(command);

                result.IsValid.Should().BeTrue();
            }

            [Theory]
            [InlineData(0)]
            [InlineData(6)]
            public void Should_Fail_When_Rating_Is_Out_Of_Range(int invalidRating)
            {
                var command = new UpdateFeedbackCommand
                {
                    Rating = invalidRating,
                    Content = "Valid content"
                };

                var result = _validator.Validate(command);

                result.IsValid.Should().BeFalse();
                result.Errors.Should().Contain(x => x.PropertyName == "Rating");
            }

            [Fact]
            public void Should_Fail_When_Content_Too_Short()
            {
                var command = new UpdateFeedbackCommand
                {
                    Rating = 3,
                    Content = "A"
                };

                var result = _validator.Validate(command);

                result.IsValid.Should().BeFalse();
                result.Errors.Should().Contain(x => x.PropertyName == "Content" && x.ErrorMessage.Contains("Minimum length"));
            }

            [Fact]
            public void Should_Fail_When_Content_Too_Long()
            {
                var longContent = new string('x', 3001);

                var command = new UpdateFeedbackCommand
                {
                    Rating = 3,
                    Content = longContent
                };

                var result = _validator.Validate(command);

                result.IsValid.Should().BeFalse();
                result.Errors.Should().Contain(x => x.PropertyName == "Content" && x.ErrorMessage.Contains("exceed"));
            }

            [Fact]
            public void Should_Pass_When_Rating_Is_Null()
            {
                var command = new UpdateFeedbackCommand
                {
                    Rating = null,
                    Content = "Valid content"
                };

                var result = _validator.Validate(command);

                result.IsValid.Should().BeTrue(); // Because Rating is nullable and not required
            }

            [Fact]
            public void Should_Pass_When_Content_Is_Null()
            {
                var command = new UpdateFeedbackCommand
                {
                    Rating = 5,
                    Content = null
                };

                var result = _validator.Validate(command);

                result.IsValid.Should().BeTrue(); // Because Content is nullable and not required
            }
        }

    }
}
