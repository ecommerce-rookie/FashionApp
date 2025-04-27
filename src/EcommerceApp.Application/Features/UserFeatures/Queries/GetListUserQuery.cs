using Application.Features.UserFeatures.Models;
using AutoMapper;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using Infrastructure.Authentication.Services;

namespace Application.Features.UserFeatures.Queries
{
    public class GetListUserQuery : IQuery<PagedList<UserPreviewResponseModel>>
    {
        public int Page { get; set; }
        public int EachPage { get; set; }
        public IEnumerable<UserRole>? Roles { get; set; }
        public string? Search { get; set; }
        public IEnumerable<UserStatus>? Statuss { get; set; }
    }

    public class GetListUserQueryHandler : IQueryHandler<GetListUserQuery, PagedList<UserPreviewResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly IAuthenticationService _authenticationService;

        public GetListUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authenticationService = authenticationService;
        }

        public async Task<PagedList<UserPreviewResponseModel>> Handle(GetListUserQuery request, CancellationToken cancellationToken)
        {
            var roleId = _authenticationService.User.Role;
            var users = new PagedList<User>();

            switch (roleId!)
            {
                case UserRole.Staff:
                {
                    request.Roles = request.Roles?.Where(x => !x.Equals(UserRole.Admin.ToString())).ToList();
                    users = await _unitOfWork.UserRepository.GetUsers(request.Page, request.EachPage, request.Roles, request.Statuss, 
                        request.Search, (UserRole)roleId!);

                    break;
                }
                case UserRole.Admin:
                {
                    users = await _unitOfWork.UserRepository.GetUsers(request.Page, request.EachPage, request.Roles, request.Statuss, 
                        request.Search, (UserRole)roleId!);
                    break;
                }
                    
            }

            return _mapper.Map<PagedList<UserPreviewResponseModel>>(users);
        }

    }
}
