using Persistence.SeedWorks.Implements;

namespace Persistence.Entity;

public partial class User : BaseAuditableEntity<Guid>
{

    public string Email { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Avatar { get; set; }

    public string Status { get; set; } = null!;

    public string? FirstName { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? Gender { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
