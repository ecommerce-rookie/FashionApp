using Application.Features.ProductFeatures.Models;
using AutoMapper;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;

namespace Application.Features.OrderFeatures.Queries
{
    public class GetBestSellerProductQuery : IQuery<IEnumerable<ProductPreviewResponseModel>>
    {
        public int EachPage { get; set; }
    }

    public class GetBestSellerProductQueryHandler : IQueryHandler<GetBestSellerProductQuery, IEnumerable<ProductPreviewResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetBestSellerProductQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductPreviewResponseModel>> Handle(GetBestSellerProductQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.ProductRepository.GetBestSeller(request.EachPage);

            return _mapper.Map<IEnumerable<ProductPreviewResponseModel>>(products);
        }

    }
}
