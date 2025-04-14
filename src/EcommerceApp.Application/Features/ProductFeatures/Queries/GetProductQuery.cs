using Application.Features.ProductFeatures.Models;
using Application.Messages;
using AutoMapper;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Infrastructure.Cache.Attributes;
using MediatR;
using System.Net;

namespace Application.Features.ProductFeatures.Queries
{
    [Cache(nameof(Product), 60 * 3)]
    public class GetProductQuery : IRequest<APIResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetProductQueryHandler : IRequestHandler<GetProductQuery, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetProductQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<APIResponse> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetDetail(request.Id);

            if(product == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound,
                    Data = request.Id
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.OK,
                Message = MessageCommon.GetSuccesfully,
                Data = _mapper.Map<ProductResponseModel>(product)
            };

        }
    }

}
