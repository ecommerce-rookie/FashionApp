using Application.Features.ProductFeatures.Models;
using Application.Messages;
using AutoMapper;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Exceptions;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Infrastructure.Cache.Attributes;
using MediatR;
using System.Net;

namespace Application.Features.ProductFeatures.Queries
{
    [Cache(nameof(Product), 60 * 3)]
    public class GetManageProductQuery : IRequest<APIResponse<ProductResponseModel>>
    {
        public string Slug { get; set; } = string.Empty;
    }

    public class GetManageProductQueryHandler : IRequestHandler<GetManageProductQuery, APIResponse<ProductResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetManageProductQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponse<ProductResponseModel>> Handle(GetManageProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetManageDetail(request.Slug);

            if(product == null)
            {
                throw new ValidationException(MessageCommon.NotFound, nameof(request.Slug));
            }

            return new APIResponse<ProductResponseModel>()
            {
                Status = HttpStatusCode.OK,
                Message = MessageCommon.GetSuccesfully,
                Data = _mapper.Map<ProductResponseModel>(product)
            };

        }
    }

}
