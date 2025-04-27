using Application.Messages;
using Domain.Aggregates.OrderAggregate.Enums;
using Domain.Exceptions;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using MediatR;
using System.Net;
using System.Text.Json.Serialization;

namespace Application.Features.OrderFeatures.Commands
{
    public class UpdateOrderStatusCommand : ICommand<APIResponse>
    {
        [JsonIgnore]
        public Guid OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }

    public class UpdateOrderStatusCommandHandler : ICommandHandler<UpdateOrderStatusCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOrderStatusCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<APIResponse> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository.GetById(request.OrderId);

            if (order == null)
            {
                throw new ValidationException(MessageCommon.NotFound, nameof(request.OrderId));
            }

            order.UpdateStatus(request.NewStatus);

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
