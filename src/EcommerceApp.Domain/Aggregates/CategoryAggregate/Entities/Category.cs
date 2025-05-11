using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Exceptions;
using Domain.SeedWorks.Abstractions;
using Persistence.SeedWorks.Implements;

namespace Domain.Aggregates.CategoryAggregate.Entities;

public partial class Category : BaseAuditableEntity<int>, IAggregateRoot
{
    public string? Name { get; private set; }

    public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category() { }

    public Category(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public Category(string name)
    {
        Name = name;
    }

    public static Category Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name cannot be empty or null", nameof(name));

        return new Category(name);
    }

    public void Update(string name)
    {
        Name = name;
    }

}
