using Application.Features.CategoryFeatures.Models;
using AutoMapper;
using Domain.Aggregates.CategoryAggregate.Entities;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using Infrastructure.Cache.Attributes;

namespace Application.Features.CategoryFeatures.Queries
{
    [Cache(nameof(Category), 60 * 3)]
    public class GetcategoryQuery : IQuery<PagedList<CategoryResponseModel>>
    {
        public int Page { get; set; }
        public int EachPage { get; set; }
        public string? Search { get; set; }
    }

    public class GetCategoryQueryHandler : IQueryHandler<GetcategoryQuery, PagedList<CategoryResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCategoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedList<CategoryResponseModel>> Handle(GetcategoryQuery request, CancellationToken cancellationToken)
        {
            if (request.Page == -1)
            {
                var result = await _unitOfWork.CategoryRepository.GetAll(c => string.IsNullOrEmpty(request.Search) || c.Name!.Contains(request.Search!));

                return new PagedList<CategoryResponseModel>(_mapper.Map<IEnumerable<CategoryResponseModel>>(result));
            } else
            {
                var result = await _unitOfWork.CategoryRepository.GetAll(c => string.IsNullOrEmpty(request.Search) || c.Name!.Contains(request.Search!), 
                    request.Page, request.EachPage);

                return _mapper.Map<PagedList<CategoryResponseModel>>(result);
            }

        }

    }
}
