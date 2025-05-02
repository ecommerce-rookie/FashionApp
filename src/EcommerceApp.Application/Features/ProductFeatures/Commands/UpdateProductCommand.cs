using Application.Features.ProductFeatures.Models;
using Application.Messages;
using Domain.Aggregates.ProductAggregate.Entities;
using Domain.Aggregates.ProductAggregate.Enums;
using Domain.Aggregates.ProductAggregate.Events;
using Domain.Aggregates.ProductAggregate.ValuesObjects;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using FluentValidation;
using Infrastructure.Shared.Extensions;
using Infrastructure.Storage;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text.Json.Serialization;
using static Infrastructure.Storage.Cloudinary.Internals.CloudinaryOptions;

namespace Application.Features.ProductFeatures.Commands
{
    public class UpdateProductCommand : ICommand<APIResponse>
    {
        [JsonIgnore]
        public string? Slug { get; set; }

        public string? Name { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? PurchasePrice { get; set; }

        public string? Description { get; set; }

        public ProductStatus? Status { get; set; }

        public int? CategoryId { get; set; }

        public int? Quantity { get; set; }

        public IEnumerable<string>? Sizes { get; set; }

        public Gender? Gender { get; set; }

        public IEnumerable<ImageFileRequestModel> Files { get; set; } = new List<ImageFileRequestModel>(); // Use for upload image

        public IEnumerable<string> Images { get; set; } = new List<string>(); // Use for delete image
    }

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .MinimumLength(3).WithMessage("Minimum length for name is 3 syllables")
                .MaximumLength(50).WithMessage("Product name cannot exceed 50 syllables")
                .MustAsync(CheckDuplicatedName).WithMessage("This product name already exist")
                .When(v => v.Name != null);

            RuleFor(v => v.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price must be assigned with a positive value")
                .When(v => v.UnitPrice.HasValue);

            RuleFor(v => v.PurchasePrice)
                .GreaterThan(0).WithMessage("Purchase price must be assigned with a positive value")
                .Must((model, purchasePrice) =>
                    purchasePrice == null || model.UnitPrice >= purchasePrice)
                .WithMessage("Unit price must be greater than or equal to purchase price");
    
            RuleFor(v => v.CategoryId)
                .MustAsync(CheckCategoryExist).WithMessage("This category does not exist");

            RuleFor(v => v.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity must be greater or equal than 0")
                .When(v => v.Quantity != null);

            RuleFor(v => v)
                .Must(v => !CheckStatusValid(v.Status, v.Quantity)).WithMessage("Status must not Available when quantity less than 0");
        }

        private bool CheckStatusValid(ProductStatus? status, int? quantity)
        {
            if (!quantity.HasValue)
            {
                return false;
            }

            return quantity == 0 && status != ProductStatus.Available;
        }

        private async Task<bool> CheckDuplicatedName(string name, CancellationToken cancellationToken)
        {
            return !await _unitOfWork.ProductRepository.CheckDuplicatedName(name);
        }

        private async Task<bool> CheckCategoryExist(int? categoryId, CancellationToken cancellationToken)
        {
            if(categoryId == null)
            {
                return true;
            }

            return await _unitOfWork.CategoryRepository.CheckCategoryExist((int)categoryId!);
        }
    }

    public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;
        private readonly IStorageService _storageService;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IPublisher publisher,
            IStorageService storageService)
        {
            _unitOfWork = unitOfWork;
            _publisher = publisher;
            _storageService = storageService;
        }
        
        public async Task<APIResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            // Check if the product exists
            var product = await _unitOfWork.ProductRepository.GetDetail(request.Slug!);
            if (product == null)
            {
                return new APIResponse
                {
                    Status = HttpStatusCode.NotFound,
                    Message = MessageCommon.NotFound
                };
            }

            // Update images if any
            if (request.Files.Any() || request.Images.Any())
            {
                await UpdateImages(product, request.Images, request.Files);
            }

            // Update product details
            product.Update(product.Id, request.Name ?? product.Name, request.UnitPrice ?? (decimal)product.Price?.UnitPrice.Amount!,
                request.PurchasePrice ?? (decimal)product.Price?.PurchasePrice.Amount!, request.Description ?? product.Description!.ToString(),
                request.Status ?? (ProductStatus)product.Status!, request.CategoryId ?? (int)product.CategoryId!, 
                request.Quantity ?? (int)product.Quantity!, request.Sizes ?? product.Sizes ?? Enumerable.Empty<string>(), 
                request.Gender ?? (Gender)product.Gender!);

            await _unitOfWork.ProductRepository.Update(product);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (result)
            {
                await _publisher.Publish(new ModifiedProductEvent(product.Id, request.Images), cancellationToken);

                return new APIResponse()
                {
                    Status = HttpStatusCode.OK,
                    Message = MessageCommon.UpdateSuccesfully
                };
            }

            return new APIResponse()
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.UpdateFailed
            };
        }

        private async Task UpdateImages(Product product, IEnumerable<string> imagesDelete, IEnumerable<ImageFileRequestModel> files)
        {
            // Delete old images in the database
            await _unitOfWork.ProductRepository.DeleteImages(product.ImageProducts!);

            // Delete old images in the storage
            product.DeleteImages(imagesDelete);

            // Get images and re-order them
            var oldImages = product.ImageProducts?
                .Where(x => !imagesDelete.Contains(x.Image.Url.ToString()))
                .OrderBy(x => x.OrderNumber)
                .Select(x => x.Image)
                .ToList() ?? new List<ImageUrl>();

            // Upload new images
            for (int i = 0; i < files.Count(); i++)
            {
                var file = files.ElementAt(i);
                var image = await _storageService.UploadImage(file.File, ImageFolder.Product, 
                    file.File.ContentType.GetEnum<ImageFormat>() ?? ImageFormat.png, string.Empty);

                // Check if the OrderNumber is within bounds
                if (file.OrderNumber < oldImages.Count)
                {
                    // Insert at the given OrderNumber
                    oldImages.Insert(file.OrderNumber, ImageUrl.Create(image.Url.ToString()));
                } else
                {
                    // If OrderNumber is out of bounds, add at the end of the list
                    oldImages.Add(ImageUrl.Create(image.Url.ToString()));
                }
            }

            // Add new images
            for (int i = 0; i < oldImages?.Count(); i++)
            {
                var image = oldImages.ElementAt(i);
                product.AddImage(image.ToString(), i + 1);
            }

            // Add new images to the database
            await _unitOfWork.ProductRepository.AddRangeImage(product.ImageProducts!);
        }

    }

}
