using Domain.Aggregates.UserAggregate.Enums;
using Domain.SeedWorks.Abstractions;
using Domain.SeedWorks.Events;
using Persistence.SeedWorks.Abstractions;

namespace Domain.Aggregates.UserAggregate.Entities
{
    public sealed class User : BaseDomainEvents<Guid>, IAggregateRoot, ISoftDelete
    {
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Phone { get; private set; }
        public string Avatar { get; private set; }
        public string Status { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public string Gender { get; private set; }
        public bool IsDeleted { get; set; }

        private User(Guid id, string email, string firstName, string lastName, string phone,
                     string avatar, string status, DateTime createdAt, DateTime? updatedAt,
                     DateTime? deletedAt, string gender)
        {
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Avatar = avatar;
            Status = status;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            DeletedAt = deletedAt;
            Gender = gender;
        }

        public static User RegisterNew(string email, string firstName, string lastName,
                                       string phone, string avatar, string gender)
        {
            var user = new User(
                Guid.NewGuid(),
                email,
                firstName,
                lastName,
                phone,
                avatar,
                status: UserStatus.NotVerify.ToString(),
                createdAt: DateTime.UtcNow,
                updatedAt: null,
                deletedAt: null,
                gender: gender
            );

            return user;
        }

        public void UpdateProfile(string firstName, string lastName, string phone, string avatar)
        {
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Avatar = avatar;
            UpdatedAt = DateTime.UtcNow;

        }

        public void ChangeStatusAccount(UserStatus userStatus)
        {
            Status = userStatus.ToString();
            DeletedAt = DateTime.UtcNow;
        }
    }
}
