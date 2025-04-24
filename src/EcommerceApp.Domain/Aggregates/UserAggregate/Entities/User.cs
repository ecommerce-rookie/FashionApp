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
        public EmailAddress Email { get; private set; } = null!;

        public string? FirstName { get; private set; }

        public string LastName { get; private set; } = null!;

        public string? Phone { get; private set; }

        public ImageUrl? Avatar { get; private set; }

        public UserStatus? Status { get; private set; }

        public virtual ICollection<Order>? Orders { get; set; } = new List<Order>();

        public virtual ICollection<Product>? Products { get; set; } = new List<Product>();

        public virtual ICollection<Feedback>? Feedbacks { get; set; } = new List<Feedback>();

        public bool IsDeleted { get; set; }

        public UserRole Role { get; set; }

        public User() { }

        public User(Guid id, string email, string? firstName, string lastName, string? phone,
                     string avatar, UserStatus status, UserRole role)
        {
            Id = id;
            Email = EmailAddress.Create(email);
            FirstName = firstName ?? string.Empty;
            LastName = lastName;
            Phone = phone ?? string.Empty;
            Avatar = ImageUrl.Create(avatar);
            Status = status;
            Role = role;
            IsDeleted = false;
        }

        public void Update(string email, string firstName, string lastName, string phone,
                     string avatar, UserStatus status)
        {
            Email = EmailAddress.Create(email);
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Avatar = ImageUrl.Create(avatar);
            Status = status;
        }

        public void ChangeStatusAccount(UserStatus userStatus)
        {
            Status = userStatus;
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
