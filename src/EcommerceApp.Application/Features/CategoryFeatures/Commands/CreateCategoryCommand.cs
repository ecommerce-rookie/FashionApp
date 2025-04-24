using Application.Messages;
using Domain.Aggregates.CategoryAggregate.Entities;
using Domain.Aggregates.CategoryAggregate.Events;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using FluentValidation;
using MediatR;
using System.Net;

namespace Application.Features.CategoryFeatures.Commands
{
    public class CreateCategoryCommand : ICommand<APIResponse>
    {
        public string Name { get; set; } = null!;
    }

    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
        }
    }

    public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IPublisher publisher)
        {
            _unitOfWork = unitOfWork;
            _publisher = publisher;
        }

        public async Task<APIResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.CategoryRepository.Add(new Category(request.Name));

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if(result)
            {
                await _publisher.Publish(new ModifiedCategoryEvent(request.Name), cancellationToken);

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
