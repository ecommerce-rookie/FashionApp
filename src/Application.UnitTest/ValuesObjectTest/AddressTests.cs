using Domain.Aggregates.UserAggregate.ValuesObject;
using Domain.Exceptions;
using FluentAssertions;

namespace Application.UnitTest.ValuesObjectTest
{
    public class AddressTests
    {
        [Fact]
        public void Create_WithValidEmail_ShouldCreateSuccessfully()
        {
            var email = "test@example.com";

            var result = EmailAddress.Create(email);

            result.Value.Should().Be(email);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("not-an-email")]
        public void Create_WithInvalidEmail_ShouldThrow(string invalidEmail)
        {
            var act = () => EmailAddress.Create(invalidEmail!);

            act.Should().Throw<ValidationException>();
        }
    }
}
