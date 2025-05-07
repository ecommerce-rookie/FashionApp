using Application.Features.ProductFeatures.Models;
using AutoMapper;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;

namespace Application.Features.ProductFeatures.Queries
{
    public class GetProductIdsQuery : IQuery<IEnumerable<ProductPreviewResponseModel>>
    {
        public IEnumerable<Guid> ProductIds { get; set; } = new List<Guid>();
    }

    public class GetProductIdsQueryHandler : IQueryHandler<GetProductIdsQuery, IEnumerable<ProductPreviewResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductIdsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductPreviewResponseModel>> Handle(GetProductIdsQuery request, CancellationToken cancellationToken)
        {
            var products = await _unitOfWork.ProductRepository.GetAll(p => request.ProductIds.Contains(p.Id), 
                p => p.ImageProducts!,
                p => p.CreatedByNavigation!
                );

            return _mapper.Map<IEnumerable<ProductPreviewResponseModel>>(products);
        }
    }

}
