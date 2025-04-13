using Application.Messages;
using Domain.Aggregates.ProductAggregate.Events;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using MediatR;
using System.Net;
using System.Text.Json.Serialization;

namespace Application.Features.ProductFeatures.Commands
{
    public class DeleteProductCommand : ICommand<APIResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public bool? Hard { get; set; }
    }

    public class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork, IPublisher publisher)
        {
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<APIResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.ProductRepository.GetById(request.Id);

            if (product == null)
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
                await _unitOfWork.ProductRepository.Delete(product);
            } else
            {
                product.Delete();
            }

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result)
            {
                await _publisher.Publish(new ModifiedProductEvent(), cancellationToken);

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
