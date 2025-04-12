using Persistence.SeedWorks.Implements;

namespace Domain.Aggregates.ProductAggregate.Entities;

public partial class Category : BaseAuditableEntity<int>
{
    public string? Name { get; private set; }

    public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category() { }

    public Category(string name)
    {
        Name = name;
    }

}
