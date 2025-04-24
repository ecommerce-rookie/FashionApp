using Application.Messages;
using Domain.Aggregates.CategoryAggregate.Events;
using Domain.Aggregates.ProductAggregate.Events;
using Domain.Exceptions;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using MediatR;
using System.Net;
using System.Text.Json.Serialization;

namespace Application.Features.CategoryFeatures.Commands
{
    public class UpdateCategoryCommand : ICommand<APIResponse>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;

        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, IPublisher publisher)
        {
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<APIResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.CategoryRepository.GetById(request.Id);

            if(category == null)
            {
                throw new NotFoundException($"Category with ID {request.Id} not found.");
            }

            category.Update(request.Name);
            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result)
            {
                await _publisher.Publish(new ModifiedCategoryEvent(category.Id, category.Name), cancellationToken);

                return new APIResponse()
                {
                    Status = HttpStatusCode.Created,
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
