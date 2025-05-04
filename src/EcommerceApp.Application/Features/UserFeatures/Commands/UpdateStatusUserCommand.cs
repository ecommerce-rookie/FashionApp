using Application.Messages;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Exceptions;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using Infrastructure.HttpClients;
using System.Net;
using System.Text.Json.Serialization;

namespace Application.Features.UserFeatures.Commands
{
    public class UpdateStatusUserCommand : ICommand<APIResponse>
    {
        [JsonIgnore]
        public Guid? UserId { get; set; }
        public UserStatus Status { get; set; }
    }

    public class UpdateStatusUserCommandHandler : ICommandHandler<UpdateStatusUserCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpService _httpService;

        public UpdateStatusUserCommandHandler(IUnitOfWork unitOfWork, IHttpService httpService)
        {
            _unitOfWork = unitOfWork;
            _httpService = httpService;
        }

        public async Task<APIResponse> Handle(UpdateStatusUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetById(request.UserId!);

            if (user == null)
            {
                throw new ValidationException(MessageCommon.NotFound, nameof(request.UserId));
            }

            user.UpdateStatus(request.Status);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result)
            {
                var response = await _httpService.UpdateStatusUser(user.Id, request.Status.ToString());

                if (response != null)
                {
                    await _unitOfWork.RollbackTransaction(cancellationToken);

                    return new APIResponse
                    {
                        Status = HttpStatusCode.InternalServerError,
                        Message = response
                    };
                }
                
                return new APIResponse
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.UpdateSuccesfully
                };
            }

            return new APIResponse
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.UpdateFailed
            };
        }
    }
}
