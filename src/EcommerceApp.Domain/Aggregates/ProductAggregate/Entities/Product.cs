using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Aggregates.ProductAggregate.ValueObjects;
using Domain.Aggregates.UserAggregate.Entities;
using Persistence.SeedWorks.Implements;

namespace Domain.Aggregates.ProductAggregate.Entities;

public partial class Product : BaseAuditableEntity<Guid>
{

    public string Name { get; set; } = null!;

    public ProductPrice? Price { get; set; }

    public string Description { get; set; } = null!;

    public ProductStatus? Status { get; set; }

    public int? CategoryId { get; set; }

    public int? Quantity { get; set; }

    public Guid? CreatedBy { get; set; }

    public string? Color { get; set; }

    public string? Size { get; set; }

    public Gender? Gender { get; set; }

    public virtual Category? Category { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<ImageProduct> ImageProducts { get; set; } = new List<ImageProduct>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
