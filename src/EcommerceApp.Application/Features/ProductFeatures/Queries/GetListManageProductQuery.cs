using Application.Features.ProductFeatures.Models;
using AutoMapper;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using Infrastructure.Cache.Attributes;

namespace Application.Features.ProductFeatures.Queries
{
    [Cache(nameof(Product), 60 * 5)]
    public class GetListManageProductQuery : IQuery<PagedList<ProductPreviewManageResponesModel>>
    {
        public int Page { get; set; }
        public int EachPage { get; set; }
        public IEnumerable<int>? Categories { get; set; }
        public string? Search { get; set; }
        public IEnumerable<string>? Sizes { get; set; }
    }

    public class GetListManageProductQueryHandler : IQueryHandler<GetListManageProductQuery, PagedList<ProductPreviewManageResponesModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetListManageProductQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedList<ProductPreviewManageResponesModel>> Handle(GetListManageProductQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.ProductRepository.GetManageProducts(request.Page, request.EachPage, request.Search,
                request.Categories, request.Sizes);

            return _mapper.Map<PagedList<ProductPreviewManageResponesModel>>(result);
        }
    }

}
