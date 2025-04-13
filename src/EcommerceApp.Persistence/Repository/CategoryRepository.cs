using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class CategoryRepository : 
        SqlRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(EcommerceContext context) : base(context)
        {
        }

        public async Task<bool> CheckCategoryExist(int categoryId)
        {
            return await _dbSet.AnyAsync(c => c.Id.Equals(categoryId));
        }
    }
}
