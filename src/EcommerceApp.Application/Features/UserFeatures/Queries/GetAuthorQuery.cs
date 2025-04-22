using Application.Features.UserFeatures.Models;
using Application.Messages;
using AutoMapper;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using Infrastructure.Authentication.Services;
using System.Net;

namespace Application.Features.UserFeatures.Queries
{
    public class GetAuthorQuery : IQuery<APIResponse>
    {
        
    }

    public class GetAuthorQueryHandler : IQueryHandler<GetAuthorQuery, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;

        public GetAuthorQueryHandler(IUnitOfWork unitOfWork, IMapper mapper,
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        public async Task<APIResponse> Handle(GetAuthorQuery request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.User.UserId;
            var author = await _unitOfWork.UserRepository.GetById(userId);

            if(author == null)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound
                };
            }
            
            return new APIResponse()
            {
                Status = HttpStatusCode.OK,
                Message = MessageCommon.GetSuccesfully,
                Data = _mapper.Map<AuthorResponseModel>(author)
            };
        }
    }

}
