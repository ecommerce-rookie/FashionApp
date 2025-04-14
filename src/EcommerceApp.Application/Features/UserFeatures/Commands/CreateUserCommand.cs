using Application.Messages;
using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using FluentValidation;
using Infrastructure.Storage;
using Microsoft.AspNetCore.Http;
using System.Net;
using static Infrastructure.Storage.Cloudinary.Internals.CloudinaryOptions;

namespace Application.Features.UserFeatures.Commands
{
    public class CreateUserCommand : ICommand<APIResponse>
    {
        public string Email { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Phone { get; set; }

        public IFormFile? Avatar { get; set; }

        public UserStatus Status { get; set; }

        public string? FirstName { get; set; }
    }

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is required and must be a valid email address.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(x => x.Phone)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number is not valid.");

            RuleFor(x => x.Avatar)
                .Must(file => file == null || (file.Length > 0 && file.Length < 2 * 1024 * 1024))
                .WithMessage("Avatar must be less than 2MB.");
        }
    }

    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork, IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _storageService = storageService;
        }

        public async Task<APIResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            string url = string.Empty;
            if(request.Avatar != null)
            {
                var resultUpload = await _storageService.UploadImage(request.Avatar, ImageFolder.Avatar, ImageFormat.png, string.Empty);
                url = resultUpload.Url.ToString();
            }

            var user = new User(Guid.NewGuid(), request.Email, request.FirstName, request.LastName, request.Phone, url, request.Status);
        
            await _unitOfWork.UserRepository.Add(user);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result)
            {
                return new APIResponse()
                {
                    Status = HttpStatusCode.Created,
                    Message = MessageCommon.CreateSuccesfully
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
