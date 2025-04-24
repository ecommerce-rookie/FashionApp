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

    public List<string>? Sizes { get; private set; } = [];

    public Gender? Gender { get; private set; }

    public virtual Category? Category { get; private set; }

    public virtual User? CreatedByNavigation { get; private set; }

    public virtual ICollection<ImageProduct>? ImageProducts { get; private set; } = new List<ImageProduct>();

    public virtual ICollection<OrderDetail>? OrderDetails { get; private set; } = new List<OrderDetail>();

    public virtual ICollection<Feedback>? Feedbacks { get; private set; } = new List<Feedback>();

    public bool IsDeleted { get; set; }

    public Product()
    {
    }

    public Product(Guid id, string name, decimal unitPrice, decimal? purchasePrice, string? description,
        ProductStatus status, int categoryId, int? quantity, List<string> sizes, Gender gender)
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
    }

    public void Update(Guid id, string name, decimal unitPrice, decimal purchasePrice, string description,
        ProductStatus status, int categoryId, int quantity, List<string> sizes, Gender gender)
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

    #region Action Feedback

    public void AddFeedback(string content, int rating, Guid userId)
    {
        var feedback = Feedback.Create(content, userId, this.Id, rating);

        Feedbacks!.Add(feedback);
    }

    public void UpdateFeedback(Guid userId, string newContent, int newRating)
    {
        var feedback = Feedbacks!.FirstOrDefault(f => f.UserId.Equals(userId) && f.ProductId.Equals(this.Id));
        if (feedback == null)
            throw new ValidationException("Feedback not found", nameof(userId));

        feedback.Update(newContent, newRating);
    }

    public void DeleteFeedback(Guid feedbackId, bool? isHard)
    {
        var feedback = Feedbacks!.FirstOrDefault(f => f.Id.Equals(feedbackId));
        if (feedback == null)
            throw new ValidationException("Feedback not found", nameof(feedbackId));

        if (isHard != null && isHard.HasValue)
            feedback.Delete();
        else
            Feedbacks!.Remove(feedback);
    }

    #endregion

    public void Delete() => IsDeleted = true;

}
