using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Domain.Exceptions;
using FluentAssertions;

namespace Application.UnitTest.Domain.ValuesObjectTest
{
    public class ImageUrlTest
    {
        [Fact]
        public void Create_WithValidUrl_ShouldCreateSuccessfully()
        {
            // Arrange
            var url = "https://example.com/image.jpg";

            // Act
            var result = ImageUrl.Create(url);

            // Assert
            result.Url.Should().Be(url);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Create_WithInvalidUrl_ShouldThrow(string? invalidUrl)
        {
            // Act
            var act = () => ImageUrl.Create(invalidUrl!);

            // Assert
            act.Should().Throw<ValidationException>()
               .WithMessage("One or more validation failures have occurred.");
        }
    }
}
