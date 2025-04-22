using Application.Messages;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using FluentValidation;
using Infrastructure.Authentication.Services;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Http;
using System.Net;
using static Infrastructure.Storage.Cloudinary.Internals.CloudinaryOptions;

namespace Application.Features.UserFeatures.Commands
{
    public class CreateUserCommand : ICommand<APIResponse>
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }

        public IFormFile? Avatar { get; set; }

        public string? Address { get; set; }
    }

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(x => x.Avatar)
                .Must(file => file == null || (file.Length > 0 && file.Length < 2 * 1024 * 1024))
                .WithMessage("Avatar must be less than 2MB.");
        }
    }

    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        private readonly IAuthenticationService _authenticationService;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork, IStorageService storageService, IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
            _authenticationService = authenticationService;
        }

        public async Task<APIResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var role = _authenticationService.User.Role;
            var email = _authenticationService.User.Email;
            var userId = _authenticationService.User.UserId;
            string url = string.Empty;

            if(request.Avatar != null)
            {
                var resultUpload = await _storageService.UploadImage(request.Avatar, ImageFolder.Avatar, ImageFormat.png, string.Empty);
                url = resultUpload.Url.ToString();
            }

            var user = new User(userId, email, request.FirstName, request.LastName!, request.Phone, url, UserStatus.Active, (UserRole)role!);
        
            await _unitOfWork.UserRepository.Add(user);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.Created,
                    Message = MessageCommon.CreateSuccesfully,
                    Data = user.Id
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.CreateFailed
            };

        }
    }
}
