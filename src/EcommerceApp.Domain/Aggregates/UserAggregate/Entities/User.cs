using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Aggregates.UserAggregate.ValuesObject;
using Domain.SeedWorks.Abstractions;
using Domain.SeedWorks.Events;
using Persistence.SeedWorks.Abstractions;

namespace Domain.Aggregates.UserAggregate.Entities
{
    public partial class User : BaseDomainEvents<Guid>, IAggregateRoot, ISoftDelete
    {
        public EmailAddress Email { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Phone { get; set; }

        public ImageUrl? Avatar { get; set; }

        public UserStatus? Status { get; set; }

        public string? FirstName { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Order>? Orders { get; set; }

        public virtual ICollection<Product>? Products { get; set; } = new List<Product>();

        public bool IsDeleted { get; set; }
        
        public User() { }

        //private User(Guid id, string email, string firstName, string lastName, string phone,
        //             string avatar, UserStatus status, DateTime createdAt, DateTime? updatedAt,
        //             DateTime? deletedAt)
        //{
        //    Id = id;
        //    Email = EmailAddress.Create(email);
        //    FirstName = firstName;
        //    LastName = lastName;
        //    Phone = phone;
        //    Avatar = ImageUrl.Create(avatar);
        //    Status = status;
        //    DeletedAt = deletedAt;
        //}

        //public static User RegisterNew(string email, string firstName, string lastName,
        //                               string phone, string avatar, string gender)
        //{
        //    var user = new User(
        //        Guid.NewGuid(),
        //        email,
        //        firstName,
        //        lastName,
        //        phone,
        //        avatar,
        //        status: UserStatus.NotVerify.ToString()
        //    );

        //    return user;
        //}

        public void UpdateProfile(string firstName, string lastName, string phone, string avatar)
        {
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Avatar = ImageUrl.Create(avatar);
            UpdatedAt = DateTime.UtcNow;

        }

        public void ChangeStatusAccount(UserStatus userStatus)
        {
            Status = userStatus;
            DeletedAt = DateTime.UtcNow;
        }
    }
}
