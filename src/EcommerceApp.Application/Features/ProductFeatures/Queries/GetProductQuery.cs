using Application.Models;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Models.Common;
using Infrastructure.Cache.Attributes;
using MediatR;

namespace Application.Features.ProductFeatures.Queries
{
    [Cache(nameof(Product), 60 * 3)]
    public class GetProductQuery : IRequest<PagedList<ProductResponseModel>>
    {

    }

    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, PagedList<ProductResponseModel>>
    {
        public Task<PagedList<ProductResponseModel>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            // Simulate fetching data from a database or other source
            var products = new List<ProductResponseModel>
            {
                new ProductResponseModel { Id = Guid.NewGuid(), Name = "Product 1" },
                new ProductResponseModel { Id = Guid.NewGuid(), Name = "Product 2" },
                new ProductResponseModel { Id = Guid.NewGuid(), Name = "Product 3" }
            };
            // Create a PagedList from the products
            var pagedList = new PagedList<ProductResponseModel>(products, products.Count, 1, 10);

            return Task.FromResult(pagedList);
        }
    }

}
