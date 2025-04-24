using Domain.Aggregates.ProductAggregate.Entities;
using Persistence.SeedWorks.Implements;

namespace Domain.Aggregates.CategoryAggregate.Entities;

public partial class Category : BaseAuditableEntity<int>
{
    public string? Name { get; private set; }

    public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category() { }

    public Category(string name)
    {
        Name = name;
    }

    public void Update(string name)
    {
        Name = name;
    }

}
