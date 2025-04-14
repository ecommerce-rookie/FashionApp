using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class ProductRepository : SqlRepository<Product>, IProductRepository
    {
        public ProductRepository(EcommerceContext context) : base(context)
        {
        }

        public async Task<bool> CheckDuplicatedName(string name)
        {
            return await _dbSet.AnyAsync(p => p.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public async Task<Product?> GetDetail(Guid id)
        {
            return await _dbSet
                .Include(p => p.ImageProducts)
                .FirstOrDefaultAsync(p => p.Id.Equals(id));
        }
    }
}
