using Application.Messages;
using Domain.Aggregates.FeedbackAggregate.Events;
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
    public class UpdateFeedbackCommand : ICommand<APIResponse>
    {
        [JsonIgnore]
        public Guid ProductId { get; set; }
        public int? Rating { get; set; }
        public string? Content { get; set; }
    }

    public class UpdateFeedbackValidator : AbstractValidator<UpdateFeedbackCommand>
    {
        public UpdateFeedbackValidator()
        {
            RuleFor(v => v.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

            RuleFor(v => v.Content)
                .MinimumLength(2).WithMessage("Minimum length for content is 2 characters")
                .MaximumLength(3000).WithMessage("Content cannot exceed 3000 characters");
        }
    }

    public class UpdateFeedbackCommandHandler : ICommandHandler<UpdateFeedbackCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;
        private readonly IAuthenticationService _authenticationService;

        public UpdateFeedbackCommandHandler(IUnitOfWork unitOfWork, IPublisher publisher, IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _publisher = publisher;
            _authenticationService = authenticationService;
        }

        public async Task<APIResponse> Handle(UpdateFeedbackCommand request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.User.UserId;
            var feedback = await _unitOfWork.FeedbackRepository.GetMyFeedback(userId, request.ProductId);

            if(feedback == null)
            {
                return new APIResponse
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound,
                    Data = request.ProductId
                };
            }

            feedback.Update(request.Content ?? feedback.Content, request.Rating ?? feedback.Rating);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if(result)
            {
                await _publisher.Publish(new ModifiedFeedbackEvent()
                {
                    Id = feedback.Id
                }, cancellationToken);

                return new APIResponse()
                {
                    Message = MessageCommon.UpdateSuccesfully,
                    Status = HttpStatusCode.OK,
                    Data = feedback.Id
                };
            }

            return new APIResponse()
            {
                Message = MessageCommon.UpdateFailed,
                Status = HttpStatusCode.InternalServerError,
                Data = request.ProductId
            };
        }
    }

}
