using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Repositories;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class CategoryRepository : 
        SqlRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(EcommerceContext context) : base(context)
        {
        }
    }
}
