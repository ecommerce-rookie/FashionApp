using Application.Features.FeedbackFeatures.Models;
using Application.Messages;
using AutoMapper;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using Infrastructure.Authentication.Services;
using System.Text.Json.Serialization;

namespace Application.Features.FeedbackFeatures.Queries
{
    public class GetFeedbackQuery : IQuery<APIResponse<FeedbackResponseModel>>
    {
        [JsonIgnore]
        public Guid ProductId { get; set; }
    }

    public class GetFeedbackQueryHandler : IQueryHandler<GetFeedbackQuery, APIResponse<FeedbackResponseModel>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;

        public GetFeedbackQueryHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        public async Task<APIResponse<FeedbackResponseModel>> Handle(GetFeedbackQuery request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.User.UserId;
            var feedback = await _unitOfWork.FeedbackRepository.GetMyFeedback(userId, request.ProductId);

            if(feedback == null)
            {
                return new APIResponse<FeedbackResponseModel>
                {
                    Status = System.Net.HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound
                };
            }

            return new APIResponse<FeedbackResponseModel>
            {
                Status = System.Net.HttpStatusCode.OK,
                Message = MessageCommon.GetSuccesfully,
                Data = _mapper.Map<FeedbackResponseModel>(feedback)
            };
        }
    }

}
