using Application.Messages;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Aggregates.UserAggregate.Events;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using Infrastructure.Storage;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json.Serialization;
using static Infrastructure.Storage.Cloudinary.Internals.CloudinaryOptions;

namespace Application.Features.UserFeatures.Commands
{
    public class UpdateUserCommand : ICommand<APIResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string? Email { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public IFormFile? Avatar { get; set; }

        public UserStatus? Status { get; set; }

        public string? FirstName { get; set; }
    }

    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        private readonly IPublisher _publisher;

        public UpdateUserCommandHandler(IUnitOfWork unitOfWork, IStorageService storageService, IPublisher publisher)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _publisher = publisher;
        }

        public async Task<APIResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetById(request.Id);

            if (user == null)
            {
                return new APIResponse
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound,
                    Data = request.Id
                };
            }

            // Update Image
            string? avatar = null;
            if (request.Avatar != null)
            {
                var imageUrl = await _storageService.UploadImage(request.Avatar, ImageFolder.Avatar, ImageFormat.png, string.Empty);
                avatar = imageUrl.Url.ToString();
            }

            user.Update(request.Email ?? user.Email.Value, request.FirstName ?? user.FirstName ?? string.Empty, request.LastName ?? user.LastName,
                request.Phone ?? user.Phone ?? string.Empty, avatar ?? user.Avatar?.Url ?? string.Empty, request.Status ?? (UserStatus)user.Status!);

            await _unitOfWork.UserRepository.Update(user);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result)
            {
                await _publisher.Publish(new ModifedUserEvent(user.Id, user.Avatar?.Url), cancellationToken);

                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.UpdateSuccesfully
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.UpdateFailed
            };
        }
    }

}
