using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Aggregates.ProductAggregate.ValueObjects;
using Domain.Aggregates.UserAggregate.Entities;
using Persistence.SeedWorks.Abstractions;
using Persistence.SeedWorks.Implements;

namespace Domain.Aggregates.ProductAggregate.Entities;

public partial class Product : BaseAuditableEntity<Guid>, ISoftDelete
{
    public string Name { get; private set; } = null!;

    public ProductPrice? Price { get; private set; }

    public string? Description { get; private set; }

    public ProductStatus Status { get; private set; }

    public int? CategoryId { get; private set; }

    public int? Quantity { get; private set; }

    public Guid? CreatedBy { get; private set; }

    public List<string> Colors { get; private set; } = [];

    public List<string> Sizes { get; private set; } = [];

    public Gender? Gender { get; private set; }

    public virtual Category? Category { get; private set; }

    public virtual User? CreatedByNavigation { get; private set; }

    public virtual ICollection<ImageProduct> ImageProducts { get; private set; } = new List<ImageProduct>();

    public virtual ICollection<OrderDetail> OrderDetails { get; private set; } = new List<OrderDetail>();

    public bool IsDeleted { get; set; }

    public Product()
    {
    }

    public Product(Guid id, string name, decimal unitPrice, decimal? purchasePrice, string? description,
        ProductStatus status, int categoryId, int? quantity, List<string>? colors, List<string> sizes, Gender gender)
    {
        Id = id;
        Name = name;
        Price = new ProductPrice(unitPrice, purchasePrice);
        Description = description;
        Status = (status == ProductStatus.Available && (quantity == null || quantity == 0) ? ProductStatus.OutOfStock : status);
        CategoryId = categoryId;
        Quantity = quantity == null ? 0 : quantity;
        Colors = colors ?? [];
        Sizes = sizes;
        Gender = gender;
    }

    public void Update(Guid id, string name, decimal unitPrice, decimal purchasePrice, string description,
        ProductStatus status, int categoryId, int quantity, List<string> colors, List<string> sizes, Gender gender)
    {
        Id = id;
        Name = name;
        Price = new ProductPrice(unitPrice, purchasePrice);
        Description = description;
        Status = (status == ProductStatus.Available && (quantity == 0) ? ProductStatus.OutOfStock : status);
        CategoryId = categoryId;
        Quantity = quantity;
        Colors = colors;
        Sizes = sizes;
        Gender = gender;
    }

    public void Delete() => IsDeleted = true;

}
