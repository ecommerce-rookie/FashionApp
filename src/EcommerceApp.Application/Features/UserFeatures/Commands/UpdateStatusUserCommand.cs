using Application.Messages;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Exceptions;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using System.Net;
using System.Text.Json.Serialization;

namespace Application.Features.UserFeatures.Commands
{
    public class UpdateStatusUserCommand : ICommand<APIResponse>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public UserStatus Status { get; set; }
    }

    public class UpdateStatusUserCommandHandler : ICommandHandler<UpdateStatusUserCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStatusUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<APIResponse> Handle(UpdateStatusUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetById(request.UserId);

            if (user == null)
            {
                throw new ValidationException(MessageCommon.NotFound, nameof(request.UserId));
            }

            user.UpdateStatus(request.Status);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result)
            {
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
