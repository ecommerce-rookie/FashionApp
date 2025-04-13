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
    }
}
