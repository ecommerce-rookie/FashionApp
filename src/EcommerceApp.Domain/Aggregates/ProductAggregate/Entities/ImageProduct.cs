using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Domain.Exceptions;

namespace Domain.Aggregates.ProductAggregate.Entities;

public partial class ImageProduct
{
    public ImageUrl Image { get; private set; } = null!;

    public Guid ProductId { get; private set; }

    public int? OrderNumber { get; private set; }

    public virtual Product? Product { get; private set; }

    public ImageProduct() { }

    public ImageProduct(string image, Guid productId, int? orderNumber)
    {
        Image = new ImageUrl(image);
        ProductId = productId;
        OrderNumber = orderNumber;
    }

    public static ImageProduct Create(string image, Guid productId, int? orderNumber)
    {
        if (string.IsNullOrWhiteSpace(image))
            throw new ValidationException("Url cannot be empty or null", nameof(image));

        return new ImageProduct(image, productId, orderNumber);
    }

}
