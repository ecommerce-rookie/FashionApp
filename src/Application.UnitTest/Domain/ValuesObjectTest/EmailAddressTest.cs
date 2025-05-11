using Domain.Aggregates.UserAggregate.ValuesObject;
using Domain.Exceptions;
using FluentAssertions;

namespace Application.UnitTest.Domain.ValuesObjectTest
{
    public class EmailAddressTest
    {
        [Theory]
        [InlineData("user@example.com")]
        [InlineData("Test.User@domain.co.uk")]
        [InlineData("admin123@sub.domain.org")]
        public void Create_WithValidEmail_ShouldReturnInstance(string input)
        {
            // Act
            var email = EmailAddress.Create(input);

            // Assert
            email.Should().NotBeNull();
            email.Value.Should().Be(input.Trim());
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Create_WithNullOrEmptyEmail_ShouldThrowValidationException(string input)
        {
            // Act
            Action act = () => EmailAddress.Create(input!);

            // Assert
            act.Should().Throw<ValidationException>()
               .WithMessage("One or more validation failures have occurred.");
        }

        [Theory]
        [InlineData("plainaddress")]
        [InlineData("missingatsign.com")]
        [InlineData("name@.com")]
        [InlineData("name@domain")]
        public void Create_WithInvalidEmailFormat_ShouldThrowValidationException(string input)
        {
            // Act
            Action act = () => EmailAddress.Create(input);

            // Assert
            act.Should().Throw<ValidationException>()
               .WithMessage("One or more validation failures have occurred.");
        }

        [Fact]
        public void Equals_TwoSameEmails_ShouldBeEqual()
        {
            // Arrange
            var email1 = EmailAddress.Create("user@example.com");
            var email2 = EmailAddress.Create("user@example.com");

            // Act & Assert
            email1.Should().Be(email2);
        }

        [Fact]
        public void Equals_TwoDifferentEmails_ShouldNotBeEqual()
        {
            // Arrange
            var email1 = EmailAddress.Create("user1@example.com");
            var email2 = EmailAddress.Create("user2@example.com");

            // Act & Assert
            email1.Should().NotBe(email2);
        }
    }
}
