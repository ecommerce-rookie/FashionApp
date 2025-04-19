using Application.Features.ProductFeatures.Enums;
using Application.Features.ProductFeatures.Models;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using Infrastructure.Cache.Attributes;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace Application.Features.ProductFeatures.Queries
{
    [Cache(nameof(Product), 60 * 5)]
    public class GetListProductQuery : IQuery<PagedList<ProductPreviewResponseModel>>
    {
        public int Page { get; set; }
        public int EachPage { get; set; }
        public ProductSortBy? SortBy { get; set; }
        public bool? IsAscending { get; set; }
        public IEnumerable<int>? Categories { get; set; }
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public IEnumerable<string>? Sizes { get; set; }
    }

    public class GetListProductQueryHandler : IQueryHandler<GetListProductQuery, PagedList<ProductPreviewResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetListProductQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<ProductPreviewResponseModel>> Handle(GetListProductQuery request,
            CancellationToken cancellationToken)
        {
            // Get the date 30 days ago from now (New products)
            var date30DaysAgo = DateTime.UtcNow.AddDays(-30);

            // Check if the request has no categories, sizes, or search term
            var hasNoCategories = request.Categories.IsNullOrEmpty();
            var hasNoSizes = request.Sizes.IsNullOrEmpty();
            var hasNoSearch = string.IsNullOrWhiteSpace(request.Search);

            // Build the predicate for filtering products
            Expression<Func<Product, bool>> predicate = p =>
                (hasNoCategories || request.Categories!.Contains((int)p.CategoryId!)) &&
                (hasNoSizes || p.Sizes.Any(size => request.Sizes!.Contains(size))) &&
                (hasNoSearch || p.Name.Contains(request.Search!)) &&
                (request.MinPrice == null || p.Price!.UnitPrice.Amount >= request.MinPrice) &&
                (request.MaxPrice == null || p.Price!.UnitPrice.Amount <= request.MaxPrice);

            // Build the selector for projecting products to ProductPreviewResponseModel
            Expression<Func<Product, ProductPreviewResponseModel>> selector = s => new ProductPreviewResponseModel
            {
                Id = s.Id,
                Name = s.Name,
                UnitPrice = s.Price!.UnitPrice.Amount,
                PurchasePrice = s.Price.PurchasePrice.Amount,
                Status = s.Status,
                Image = s.ImageProducts
                                    .Where(i => i.OrderNumber == 1)
                                    .Select(i => i.Image.Url)
                                    .FirstOrDefault(),
                IsNew = s.CreatedAt >= date30DaysAgo
            };

            // Apply sorting
            var result = await _unitOfWork.ProductRepository.GetAll(
                predicate,
                selector,
                request.Page,
                request.EachPage,
                request.SortBy?.ToString() ?? ProductSortBy.CreatedAt.ToString(),
                request.IsAscending ?? true
            );

            return result;
        }
    }

}
