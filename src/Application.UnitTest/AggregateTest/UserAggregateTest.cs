using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using FluentAssertions;

namespace Application.UnitTest.AggregateTest
{
    public class UserAggregateTest
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateUserCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var email = "test@example.com";
            var firstName = "John";
            var lastName = "Doe";
            var phone = "123456789";
            var avatar = "https://example.com/avatar.png";
            var status = UserStatus.Active;
            var role = UserRole.Customer;

            // Act
            var user = new User(id, email, firstName, lastName, phone, avatar, status, role);

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().Be(id);
            user.Email.Value.Should().Be(email);
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
            user.Phone.Should().Be(phone);
            user.Avatar?.Url.Should().Be(avatar);
            user.Status.Should().Be(status);
            user.Role.Should().Be(role);
            user.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public void Update_WithValidData_ShouldUpdateProperties()
        {
            // Arrange
            var user = new User(
                Guid.NewGuid(),
                "old@example.com",
                "OldFirst",
                "OldLast",
                "0000000000",
                "https://old.com/avatar.png",
                UserStatus.NotVerify,
                UserRole.Admin
            );

            var newEmail = "new@example.com";
            var newFirstName = "NewFirst";
            var newLastName = "NewLast";
            var newPhone = "1111111111";
            var newAvatar = "https://new.com/avatar.png";
            var newStatus = UserStatus.Active;

            // Act
            user.Update(newEmail, newFirstName, newLastName, newPhone, newAvatar, newStatus);

            // Assert
            user.Email.Value.Should().Be(newEmail);
            user.FirstName.Should().Be(newFirstName);
            user.LastName.Should().Be(newLastName);
            user.Phone.Should().Be(newPhone);
            user.Avatar?.Url.Should().Be(newAvatar);
            user.Status.Should().Be(newStatus);
        }

        [Fact]
        public void UpdateStatus_ShouldChangeUserStatus()
        {
            // Arrange
            var user = new User(
                Guid.NewGuid(),
                "user@example.com",
                "First",
                "Last",
                "123456",
                "https://avatar.png",
                UserStatus.NotVerify,
                UserRole.Customer
            );

            var newStatus = UserStatus.Banned;

            // Act
            user.UpdateStatus(newStatus);

            // Assert
            user.Status.Should().Be(newStatus);
        }

        [Fact]
        public void Delete_ShouldMarkUserAsDeleted()
        {
            // Arrange
            var user = new User(
                Guid.NewGuid(),
                "user@example.com",
                "First",
                "Last",
                "123456",
                "https://avatar.png",
                UserStatus.Active,
                UserRole.Customer
            );

            // Act
            user.Delete();

            // Assert
            user.IsDeleted.Should().BeTrue();
        }
    }
}
