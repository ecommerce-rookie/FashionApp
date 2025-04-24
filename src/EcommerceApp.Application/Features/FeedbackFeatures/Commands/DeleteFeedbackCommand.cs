using Application.Messages;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Events;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using MediatR;
using System.Net;

namespace Application.Features.FeedbackFeatures.Commands
{
    public class DeleteFeedbackCommand : ICommand<APIResponse>
    {
        public Guid Id { get; set; }
        public bool? Hard { get; set; }
    }

    public class DeleteFeedbackCommandHandler : ICommandHandler<DeleteFeedbackCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;

        public DeleteFeedbackCommandHandler(IUnitOfWork unitOfWork, IPublisher publisher)
        {
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<APIResponse> Handle(DeleteFeedbackCommand request, CancellationToken cancellationToken)
        {
            var feedback = await _unitOfWork.FeedbackRepository.GetById(request.Id);
            if (feedback == null)
            {
                return new APIResponse
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound,
                    Data = request.Id
                };
            }

            if (request.Hard.HasValue && request.Hard.Value)
            {
                await _unitOfWork.FeedbackRepository.Delete(feedback);
            } else
            {
                feedback.Delete();
            }

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);
            if (result)
            {
                await _publisher.Publish(new ModifiedFeedbackEvent(), cancellationToken);

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
