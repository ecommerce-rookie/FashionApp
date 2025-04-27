using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Models.Common;
using Domain.Repositories;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class ProductRepository : SqlRepository<Product>, IProductRepository
    {
        private readonly EcommerceContext _context;

        public ProductRepository(EcommerceContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CheckDuplicatedName(string name)
        {
            return await _dbSet.AnyAsync(p => p.Name.Equals(name));
        }

        public async Task<Product?> GetDetail(string slug)
        {
            return await _dbSet
                .AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.CreatedByNavigation)
                .Include(p => p.Category)
                .Include(p => p.ImageProducts)
                .Include(p => p.Feedbacks)
                .FirstOrDefaultAsync(p => p.Slug!.Equals(slug));
        }

        public async Task<Product?> GetManageDetail(string slug)
        {
            return await _dbSet
                .AsNoTracking()
                .AsSplitQuery()
                .IgnoreQueryFilters()
                .Include(p => p.CreatedByNavigation)
                .Include(p => p.Category)
                .Include(p => p.ImageProducts)
                .Include(p => p.Feedbacks)
                .Include(p => p.CreatedByNavigation)
                .FirstOrDefaultAsync(p => p.Slug!.Equals(slug));
        }

        public async Task<IEnumerable<Product>> GetRecommendProduct(string slug, int eachPage)
        {
            var product = await _dbSet.AsNoTracking().AsSplitQuery().FirstOrDefaultAsync(p => p.Slug!.Equals(slug));

            if(product == null)
            {
                return Enumerable.Empty<Product>();
            }

            return await _dbSet
                .AsNoTracking()
                .AsSplitQuery()
                .Where(p => !product.Id.Equals(p.Id) && product.CategoryId.Equals(p.CategoryId))
                .OrderByDescending(p => p.Gender.Equals(product.Gender))
                    .ThenByDescending(p => p.CreatedAt)
                .Take(eachPage)
                .Include(p => p.ImageProducts)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetBestSeller(int eachPage)
        {
            var best = await _context.OrderDetails
                .Where(od => od.ProductId != null)
                .GroupBy(od => od.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key!.Value,
                    TotalSold = g.Sum(x => x.Quantity ?? 0),
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(eachPage)
                .Join(
                    _context.Products.Include(p => p.ImageProducts),
                    g => g.ProductId,
                    p => p.Id,
                    (g, p) => p
                )
                .ToListAsync();

            return best;
        }

        public async Task DeleteImages(IEnumerable<ImageProduct> images)
        {
            _context.ImageProducts.RemoveRange(images);

            await Task.CompletedTask;
        }

        public async Task<PagedList<Product>> GetManageProducts(int page, int eachPage, string? search, 
            IEnumerable<int>? categories, IEnumerable<string>? sizes)
        {
            var query = _dbSet
                .AsNoTracking()
                .AsQueryable()
                .IgnoreQueryFilters();

            if (categories != null && categories.Any())
            {
                query = query.Where(p => categories.Contains(p.CategoryId ?? 0));
            }

            if (sizes != null && sizes.Any())
            {
                query = query.Where(p => p.Sizes != null && p.Sizes.Any(size => sizes.Contains(size)));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }

            query = query
                .Include(p => p.Category)
                .Include(p => p.ImageProducts)
                .Include(p => p.Feedbacks)
                .OrderByDescending(p => p.CreatedAt);

            return await query.ToPagedListAsync(page, eachPage);
        }

    }
}
