using Microsoft.EntityFrameworkCore;
using Persistence.Extensions;
using Persistence.SeedWorks.Abstractions;

namespace Application.UnitTest.Persistance.Extensions
{
    public class ModelBuilderExtensionTest
    {
        private class SoftDeletableEntity : ISoftDelete
        {
            public bool IsDeleted { get; set; }
        }

        [Fact]
        public void ApplySoftDeleteQueryFilter_AddsFilterToSoftDeleteEntities()
        {
            var options = new DbContextOptionsBuilder<DbContext>()
                //.UseInMemoryDatabase("TestDb")
                .Options;

            using var context = new DbContext(options);
            var modelBuilder = new ModelBuilder();

            modelBuilder.Entity<SoftDeletableEntity>();
            modelBuilder.ApplySoftDeleteQueryFilter();

            var model = modelBuilder.Model;
            var entity = model.FindEntityType(typeof(SoftDeletableEntity));
            var queryFilter = entity.GetQueryFilter();

            Assert.NotNull(queryFilter);
            Assert.Contains("IsDeleted", queryFilter.ToString());
        }
    }
}
