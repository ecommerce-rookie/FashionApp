using Application.Messages;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Aggregates.ProductAggregate.Events;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using FluentValidation;
using Infrastructure.Authentication.Services;
using Infrastructure.Shared.Extensions;
using Infrastructure.Storage;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;
using static Infrastructure.Storage.Cloudinary.Internals.CloudinaryOptions;

namespace Application.Features.ProductFeatures.Commands
{
    public class CreateProductCommand : ICommand<APIResponse>
    {
        public string? Name { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal? PurchasePrice { get; set; }

        public string? Description { get; set; }

        public ProductStatus Status { get; set; }

        public int CategoryId { get; set; }

        public int? Quantity { get; set; }

        public List<string>? Sizes { get; set; }

        public Gender Gender { get; set; }

        public IEnumerable<IFormFile> Files { get; set; } = new List<IFormFile>();
    }

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Product name is required")
                .MinimumLength(3).WithMessage("Minimum length for name is 3 syllables")
                .MaximumLength(50).WithMessage("Product name cannot exceed 50 syllables")
                .MustAsync(CheckDuplicatedName).WithMessage("This product name already exist");

            RuleFor(v => v.UnitPrice)
                .NotNull().WithMessage("Unit price is required")
                .GreaterThan(0).WithMessage("Unit price must be asigned with a positive value");

            RuleFor(v => v.PurchasePrice)
                .GreaterThan(0).WithMessage("Purchase price must be asigned with a positive value")
                .Must((model, purchasePrice) => model.UnitPrice >= (purchasePrice ?? 0))
                .WithMessage("Unit price must be greater than or equal to purchase price");

            RuleFor(v => v.CategoryId)
                .NotNull().WithMessage("Category is required")
                .MustAsync(CheckCategoryExist).WithMessage("This category does not exist");

            RuleFor(v => v.Quantity)
                .NotNull().WithMessage("Quantity is required")
                .GreaterThanOrEqualTo(0).WithMessage("Quantity must be greater or equal than 0");

            RuleFor(v => v)
                .Must(v => CheckStatusValid(v.Status, v.Quantity)).WithMessage("Status must be Available when quantity greater than 0");
        }

        private bool CheckStatusValid(ProductStatus? status, int? quantity)
        {
            if(!quantity.HasValue)
            {
                return false;
            }

            return !(quantity == 0 && status == ProductStatus.Available);
        }

        private async Task<bool> CheckDuplicatedName(string name, CancellationToken cancellationToken)
        {
            return !await _unitOfWork.ProductRepository.CheckDuplicatedName(name);
        }

        private async Task<bool> CheckCategoryExist(int categoryId, CancellationToken cancellationToken)
        {
            return await _unitOfWork.CategoryRepository.CheckCategoryExist((int)categoryId!);
        }
    }

    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        private readonly IStorageService _storageService;
        private readonly IPublisher _publisher;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService, 
                                IStorageService storageService, IPublisher publisher)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _storageService = storageService;
            _publisher = publisher;
        }

        public async Task<APIResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.User.UserId;

            if (_authenticationService.User.Role != UserRole.Staff)
            {
                return new APIResponse()
                {
                    Message = MessageCommon.Forbidden,
                    Status = HttpStatusCode.Forbidden,
                };
            }

            var product = new Product(Guid.NewGuid(), request.Name!, request.UnitPrice, request.PurchasePrice, request.Description,
                request.Status, request.CategoryId, request.Quantity, request.Sizes!, request.Gender);

            await _unitOfWork.ProductRepository.Add(product);

            for (int i = 0; i < request.Files.Count(); i++)
            {
                var image = await _storageService.UploadImage(request.Files.ElementAt(i), ImageFolder.Product, request.Files.ElementAt(i).ContentType.GetEnum<ImageFormat>() ?? ImageFormat.png, string.Empty);
                product.AddImage(image.Url.ToString(), i + 1);
            }

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result)
            {
                await _publisher.Publish(new CreatedProductEvent(), cancellationToken);

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
