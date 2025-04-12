using Domain.Aggregates.ProductAggregate.ValuesObjects;

namespace Domain.Aggregates.ProductAggregate.Entities;

public partial class ImageProduct
{
    public ImageUrl Image { get; set; } = null!;

    public Guid ProductId { get; set; }

    public int? OrderNumber { get; set; }

    public virtual Product Product { get; set; } = null!;
}
