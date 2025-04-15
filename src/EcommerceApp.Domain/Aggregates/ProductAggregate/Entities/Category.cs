using Persistence.SeedWorks.Implements;
using System.Text.Json.Serialization;

namespace Domain.Aggregates.ProductAggregate.Entities;

public partial class Category : BaseAuditableEntity<int>
{
    public string? Name { get; private set; }

    //[JsonIgnore]
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
