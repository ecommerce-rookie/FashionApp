using Application.Features.ProductFeatures.Models;
using AutoMapper;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;

namespace Application.Features.ProductFeatures.Queries
{
    public class GetRecommendProductQuery : IQuery<IEnumerable<ProductPreviewResponseModel>>
    {
        public string Slug { get; set; } = string.Empty;
        public int EachPage { get; set; }
    }

    public class GetRecommendProductQueryHandler : IQueryHandler<GetRecommendProductQuery, IEnumerable<ProductPreviewResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetRecommendProductQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ProductPreviewResponseModel>> Handle(GetRecommendProductQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.ProductRepository.GetRecommendProduct(request.Slug, request.EachPage);

            return _mapper.Map<IEnumerable<ProductPreviewResponseModel>>(products);
        }
    }

}
