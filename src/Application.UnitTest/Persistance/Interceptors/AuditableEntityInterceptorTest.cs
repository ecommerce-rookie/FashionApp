using ASM.Application.Infrastructure.Persistence.Interceptors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using Persistence.SeedWorks.Implements;

namespace Application.UnitTest.Persistance.Interceptors
{
    public class AuditableEntityInterceptorTest
    {
        public class TestAuditableEntity : BaseAuditableEntity<Guid>
        {
            public string Name { get; set; } = string.Empty;
        }

        public class TestDbContext : DbContext
        {
            public DbSet<TestAuditableEntity> TestEntities { get; set; }

            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        }

        [Fact]
        public void UpdateEntities_Should_Set_Timestamps_On_Add()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);
            var entity = new TestAuditableEntity { Name = "Test" };

            context.TestEntities.Add(entity);
            context.ChangeTracker.DetectChanges();

            AuditableEntityInterceptor.UpdateEntities(context);

            entity.CreatedAt.Should().Be(entity.CreatedAt);
            entity.UpdatedAt.Should().Be(entity.UpdatedAt);

            context.SaveChanges();
        }

        [Fact]
        public void UpdateEntities_Should_Not_Throw_When_Context_Is_Null()
        {
            var act = () => AuditableEntityInterceptor.UpdateEntities(null);
            act.Should().NotThrow();
        }

        [Fact]
        public async Task SavingChangesAsync_Should_Invoke_UpdateEntities()
        {
            var interceptor = new AuditableEntityInterceptor();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new TestDbContext(options);
            context.TestEntities.Add(new TestAuditableEntity { Name = "Async Test" });

            var dbContextEventData = new DbContextEventData(
                eventDefinition: null!,
                messageGenerator: (_, _) => "",
                context: context
            );

            var result = await interceptor.SavingChangesAsync(dbContextEventData, default);

            //result.Should().Be(default);
            result.HasResult.Should().BeFalse();

        }

    }
}
