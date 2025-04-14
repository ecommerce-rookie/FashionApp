using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class ImageProductRepository : SqlRepository<ImageProduct>, IImageProductRepository
    {
        public ImageProductRepository(EcommerceContext context) : base(context)
        {
        }

        public async Task DeleteRange(IEnumerable<ImageProduct> images)
        {
            _dbSet.RemoveRange(images);

            await Task.CompletedTask;
        }

    }
}
