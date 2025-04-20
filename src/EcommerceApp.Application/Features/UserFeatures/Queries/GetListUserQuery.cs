using Application.Features.UserFeatures.Models;
using AutoMapper;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;

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

        public GetListUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedList<UserPreviewResponseModel>> Handle(GetListUserQuery request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.UserRepository.GetUsers(request.Page, request.EachPage, request.Roles, request.Statuss, request.Search);
        
            return _mapper.Map<PagedList<UserPreviewResponseModel>>(users);
        }

    }
}
