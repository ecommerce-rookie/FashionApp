using Domain.Aggregates.CategoryAggregate.Entities;
using Domain.Aggregates.FeedbackAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Aggregates.ProductAggregate.ValueObjects;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Exceptions;
using Domain.SeedWorks.Abstractions;
using Persistence.SeedWorks.Abstractions;
using Persistence.SeedWorks.Implements;

namespace Domain.Aggregates.ProductAggregate.Entities;

public partial class Product : BaseAuditableEntity<Guid>, IAggregateRoot, ISoftDelete
{
    public string Name { get; private set; } = null!;

    public ProductPrice? Price { get; private set; }

    public string? Description { get; private set; }

    public ProductStatus Status { get; private set; }

    public int? CategoryId { get; private set; }

    public int? Quantity { get; private set; }

    public string? Slug { get; private set; }

    public Guid? CreatedBy { get; private set; }

    public IEnumerable<string>? Sizes { get; private set; } = [];

    public Gender? Gender { get; private set; }

    public virtual Category? Category { get; private set; }

    public virtual User? CreatedByNavigation { get; private set; }

    public virtual ICollection<ImageProduct>? ImageProducts { get; private set; }

    public virtual ICollection<OrderDetail>? OrderDetails { get; private set; }

    public virtual ICollection<Feedback>? Feedbacks { get; private set; }

    public bool IsDeleted { get; set; }

    public Product()
    {
    }

    public Product(Guid id, string name, decimal unitPrice, decimal? purchasePrice, string? description,
        ProductStatus status, int categoryId, int? quantity, List<string> sizes, Gender gender, Guid createdBy)
    {
        Id = id;
        Name = name;
        Price = new ProductPrice(unitPrice, purchasePrice);
        Description = description;
        Status = (status == ProductStatus.Available && (quantity == null || quantity == 0) ? ProductStatus.OutOfStock : status);
        CategoryId = categoryId;
        Quantity = quantity == null ? 0 : quantity;
        Sizes = sizes;
        Gender = gender;
        Slug = $"{SlugHelper.Generate(name)}-{id}";
        IsDeleted = false;
        CreatedBy = createdBy;
    }

    public static Product Create(Guid id, string name, decimal unitPrice, decimal? purchasePrice, string? description,
        ProductStatus status, int categoryId, int? quantity, List<string> sizes, Gender gender, Guid createdBy)
    {
        if(name.Length <= 2)
            throw new ValidationException("Name must be at least 2 characters long", nameof(name));

        if (unitPrice < 0)
            throw new ValidationException("Unit price cannot be negative", nameof(unitPrice));

        if (purchasePrice < 0)
            throw new ValidationException("Purchase price cannot be negative", nameof(purchasePrice));

        if (purchasePrice > unitPrice)
            throw new ValidationException("Purchase price cannot be greater than unit price", nameof(unitPrice));

        if(quantity < 0)
            throw new ValidationException("Quantity cannot be negative", nameof(quantity));

        return new Product(id, name, unitPrice, purchasePrice, description, status, categoryId, quantity, sizes, gender, createdBy);

    }

    public void Update(Guid id, string name, decimal unitPrice, decimal purchasePrice, string description,
        ProductStatus status, int categoryId, int quantity, IEnumerable<string> sizes, Gender gender)
    {
        Id = id;
        Name = name;
        Price = new ProductPrice(unitPrice, purchasePrice);
        Description = description;
        Status = (status == ProductStatus.Available && (quantity == 0) ? ProductStatus.OutOfStock : status);
        CategoryId = categoryId;
        Quantity = quantity;
        Sizes = sizes;
        Gender = gender;
        Slug = $"{SlugHelper.Generate(name)}-{id}";
    }

    #region Action Images

    public void AddImage(string imageUrl, int orderNumber)
    {
        var newImage = ImageProduct.Create(imageUrl, this.Id, orderNumber);

        ImageProducts!.Add(newImage);
    }

    public void DeleteImage(string url)
    {
        var image = ImageProducts!.FirstOrDefault(i => i.Image.Url.Equals(url));
        if (image == null)
            throw new ValidationException("Image not found", nameof(url));

        ImageProducts!.Remove(image);
    }

    public void DeleteImages(IEnumerable<string> urls)
    {
        foreach (var url in urls)
        {
            DeleteImage(url);
        }
    }

    #endregion

    public void Delete() => IsDeleted = true;

}
