using Microsoft.EntityFrameworkCore;
using Persistence.SeedWorks.Abstractions;
using System.Linq.Expressions;
using System.Reflection;

namespace Persistence.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
        {
            // Get all entity types in the model
            var entityTypes = modelBuilder.Model.GetEntityTypes();

            foreach (var entityType in entityTypes)
            {
                // Check if the entity implements ISoftDelete
                var clrType = entityType.ClrType;
                if (typeof(ISoftDelete).IsAssignableFrom(clrType))
                {
                    // Get the IsDeleted property
                    var isDeletedProperty = clrType.GetProperty("IsDeleted", BindingFlags.Public | BindingFlags.Instance);

                    if (isDeletedProperty != null && isDeletedProperty.PropertyType == typeof(bool))
                    {
                        // Build the query filter expression
                        var parameter = Expression.Parameter(clrType, "e");
                        var filter = Expression.Lambda(
                            Expression.Equal(
                                Expression.Property(parameter, isDeletedProperty),
                                Expression.Constant(false)
                            ),
                            parameter
                        );

                        // Apply the query filter
                        modelBuilder.Entity(clrType).HasQueryFilter(filter);
                    }
                }
            }
        }
    }
}
