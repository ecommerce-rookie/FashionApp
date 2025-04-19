using Application.Features.OrderFeatures.Models;
using AutoMapper;
using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Enums;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using Infrastructure.Authentication.Services;
using static Application.Features.OrderFeatures.Enums.OrderEnums;

namespace Application.Features.OrderFeatures.Queries
{
    public class GetListOrderQuery : IQuery<PagedList<OrderResponseModel>>
    {
        public int Page { get; set; }
        public int EachPage { get; set; }
    }

    public class GetListOrderQueryHandler : IQueryHandler<GetListOrderQuery, PagedList<OrderResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;

        public GetListOrderQueryHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        public async Task<PagedList<OrderResponseModel>> Handle(GetListOrderQuery request, CancellationToken cancellationToken)
        {
            var roleId = _authenticationService.User.Role;
            var userId = _authenticationService.User.UserId;

            var orders = new PagedList<Order>();

            if (roleId == UserRole.Customer)
            {
                orders = await _unitOfWork.OrderRepository.GetAll(o => 
                    (o.CustomerId.Equals(userId)),
                    request.Page, request.EachPage,
                    OrderSortBy.CreatedAt.ToString(),
                    true,
                    o => o.OrderDetails!
                );

            } else if(roleId == UserRole.Admin && roleId == UserRole.Staff)
            {
                orders = await _unitOfWork.OrderRepository.GetAll(
                    request.Page, request.EachPage,
                    OrderSortBy.CreatedAt.ToString(),
                    true,
                    o => o.Customer!,
                    o => o.OrderDetails!
                );

            }

            return _mapper.Map<PagedList<OrderResponseModel>>(orders);

        }

    }
}
