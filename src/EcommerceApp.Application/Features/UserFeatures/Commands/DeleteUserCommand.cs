using Application.Messages;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using System.Net;

namespace Application.Features.UserFeatures.Commands
{
    public class DeleteUserCommand : ICommand<APIResponse>
    {
        public Guid Id { get; set; }
        public bool? Hard { get; set; }
    }

    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<APIResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetById(request.Id);

            if(user == null)
            {
                return new APIResponse
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound,
                    Data = request.Id
                };
            }

            if (request.Hard.HasValue)
            {
                await _unitOfWork.UserRepository.Delete(user);
            } else
            {
                user.Delete();
            }

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.DeleteSuccessfully
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.DeleteFailed
            };

        }
    }

}
