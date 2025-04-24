using Domain.Aggregates.CategoryAggregate.Entities;
using Domain.Aggregates.FeedbackAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.SeedWorks.Abstractions;
using Domain.SeedWorks.Events;
using Microsoft.EntityFrameworkCore;
using Persistence.SeedWorks.Abstractions;
using System.Collections.Immutable;

namespace Persistence.Contexts
{
    public partial class EcommerceContext : DbContext, IDomainEventContext, IEcommerceContext
    {
        public EcommerceContext()
        {
        }

        public EcommerceContext(DbContextOptions<EcommerceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<ImageProduct> ImageProducts { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }

        public IEnumerable<BaseEvent> GetDomainEvents()
        {
            var domainEntities = ChangeTracker
                .Entries<IBaseDomainEvent>()
                .Where(x => x.Entity.DomainEvents.Count != 0)
                .ToImmutableList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToImmutableList();

            domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

            return domainEvents;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=ep-lucky-salad-a1n7zc5c-pooler.ap-southeast-1.aws.neon.tech;Port=5432;Database=ecommerce;Username=ecommerce_owner;Password=npg_xLb9HfjNu5mg;Pooling=true;SSL Mode=Require;Trust Server Certificate=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.DbContextAssembly);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
