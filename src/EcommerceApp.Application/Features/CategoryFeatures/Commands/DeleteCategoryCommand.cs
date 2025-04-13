using Application.Messages;
using Domain.Aggregates.ProductAggregate.Events;
using Domain.Exceptions;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.CategoryFeatures.Commands
{
    public class DeleteCategoryCommand : ICommand<APIResponse>
    {
        public int Id { get; set; }
    }

    public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, IPublisher publisher)
        {
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<APIResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.CategoryRepository.GetById(request.Id);

            if (category == null)
            {
                throw new NotFoundException($"Category with ID {request.Id} not found.");
            }

            await _unitOfWork.CategoryRepository.Delete(category);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result)
            {
                await _publisher.Publish(new ModifiedCategoryEvent(category.Id, category.Name), cancellationToken);

                return new APIResponse()
                {
                    Status = HttpStatusCode.Created,
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
