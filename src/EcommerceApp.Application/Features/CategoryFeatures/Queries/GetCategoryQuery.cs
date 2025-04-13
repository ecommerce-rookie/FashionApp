using Application.Features.CategoryFeatures.Models;
using Application.Messages;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using Infrastructure.Cache.Attributes;
using System.Net;

namespace Application.Features.CategoryFeatures.Queries
{
    [Cache(nameof(Category), 60 * 3)]
    public class GetcategoryQuery : IQuery<IEnumerable<CategoryResponseModel>>
    {
    }

    public class GetCategoryQueryHandler : IQueryHandler<GetcategoryQuery, IEnumerable<CategoryResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCategoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CategoryResponseModel>> Handle(GetcategoryQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.CategoryRepository.GetAll();

            var response = result.Select(c => new CategoryResponseModel
            {
                Id = c.Id,
                Name = c.Name!
            }).ToList();

            return response;
        }

    }
}
