using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repository
{
    public class ImageProductRepository : SqlRepository<ImageProduct>, IImageProductRepository
    {
        public ImageProductRepository(DbContext context) : base(context)
        {
        }
    }
}
