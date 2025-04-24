using Application.Messages;
using Domain.Aggregates.FeedbackAggregate.Entities;
using Domain.Aggregates.FeedbackAggregate.Events;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Events;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using FluentValidation;
using Infrastructure.Authentication.Services;
using MediatR;
using System.Net;
using System.Text.Json.Serialization;

namespace Application.Features.FeedbackFeatures.Commands
{
    public class CreateFeedbackCommand : ICommand<APIResponse>
    {
        [JsonIgnore]
        public Guid ProductId { get; set; }
        public int? Rating { get; set; }
        public string? Content { get; set; }
    }

    public class CreateFeedbackValidator : AbstractValidator<CreateFeedbackCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;

        public CreateFeedbackValidator(IUnitOfWork unitOfWork, IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Rating)
                .NotNull().WithMessage("Rating is required")
                .NotEmpty().WithMessage("Rating is required")
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

            RuleFor(v => v.Content)
                .NotNull().WithMessage("Content is required")
                .NotEmpty().WithMessage("Content is required")
                .MinimumLength(2).WithMessage("Minimum length for content is 2 characters")
                .MaximumLength(3000).WithMessage("Content cannot exceed 3000 characters");

            RuleFor(v => v.ProductId)
                .NotNull().WithMessage("ProductId is required")
                .NotEmpty().WithMessage("ProductId is required")
                .MustAsync(CheckUserFeedbackExist).WithMessage("You have already given feedback for this product");
        }

        private async Task<bool> CheckUserFeedbackExist(Guid productId, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.User.UserId;

            return !await _unitOfWork.FeedbackRepository.CheckExistUserInProduct(userId, productId);
        }

    }

    public class CreateFeedbackCommandHandler : ICommandHandler<CreateFeedbackCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPublisher _publisher;

        public CreateFeedbackCommandHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService,
            IPublisher publisher)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _publisher = publisher;
        }

        public async Task<APIResponse> Handle(CreateFeedbackCommand request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.User.UserId;

            var feedback = new Feedback(Guid.NewGuid(), request.Content!, userId, request.ProductId, (int)request.Rating!);

            await _unitOfWork.FeedbackRepository.Add(feedback);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if(result)
            {
                await _publisher.Publish(new ModifiedFeedbackEvent() {
                    Id = feedback.Id
                }, cancellationToken);

                return new APIResponse()
                {
                    Message = MessageCommon.CreateSuccesfully,
                    Status = HttpStatusCode.Created
                };
            }

            return new APIResponse()
            {
                Message = MessageCommon.CreateFailed,
                Status = HttpStatusCode.InternalServerError,
            };
        }
    }

}
