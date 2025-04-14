using Application.Features.ProductFeatures.Enums;
using Application.Features.ProductFeatures.Models;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using Infrastructure.Cache.Attributes;
using Microsoft.IdentityModel.Tokens;

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
            var query = await _unitOfWork.ProductRepository.GetAll(p => 
                (request.Categories.IsNullOrEmpty() || !request.Categories!.Contains((int)p.CategoryId!)) &&
                (request.Sizes.IsNullOrEmpty()) || !p.Sizes.Any(size => request.Sizes!.Contains(size)) &&
                (request.Search.IsNullOrEmpty() || !p.Name.Contains(request.Search!)) &&
                (request.MinPrice == null || p.Price!.UnitPrice.Amount >= request.MinPrice) &&
                (request.MaxPrice == null || p.Price!.UnitPrice.Amount <= request.MaxPrice),
                (s => new ProductPreviewResponseModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    UnitPrice = s.Price!.UnitPrice.Amount,
                    PurchasePrice = s.Price.PurchasePrice.Amount,
                    Status = s.Status,
                    Image = s.ImageProducts.Where(i => i.OrderNumber == 1).Select(i => i.Image.Url).FirstOrDefault(),
                    IsNew = s.CreatedAt >= DateTime.UtcNow.AddDays(-30)
                }),
                request.Page,
                request.EachPage,
                request.SortBy.ToString() ?? ProductSortBy.Newest.ToString(),
                request?.IsAscending ?? true
            );

            return query;
        }
    }

}
