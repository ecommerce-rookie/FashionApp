using Domain.Aggregates.CategoryAggregate.Entities;
using Domain.Aggregates.FeedbackAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Contexts
{
    public interface IEcommerceContext
    {
        DbSet<Category> Categories { get; set; }

        DbSet<ImageProduct> ImageProducts { get; set; }

        DbSet<Order> Orders { get; set; }

        DbSet<OrderDetail> OrderDetails { get; set; }

        DbSet<Product> Products { get; set; }

        DbSet<User> Users { get; set; }

        DbSet<Feedback> Feedbacks { get; set; }
    }
}
