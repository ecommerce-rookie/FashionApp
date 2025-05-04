using Application.Messages;
using Domain.Aggregates.ProductAggregate.Events;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using MediatR;
using System.Net;

namespace Application.Features.ProductFeatures.Commands
{
    public class DeleteProductCommand : ICommand<APIResponse>
    {
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
            var product = await _unitOfWork.ProductRepository.GetManageById(request.Id);

            if (product == null)
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
                await _unitOfWork.ProductRepository.Delete(product);
            } else
            {
                product.Delete();
            }

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result)
            {
                await _publisher.Publish(new ModifiedProductEvent()
                {
                    Images = product.ImageProducts?.Select(i => i.Image.Url).ToList(),
                    IsPermanently = request.Hard ?? false,
                }, cancellationToken);

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
