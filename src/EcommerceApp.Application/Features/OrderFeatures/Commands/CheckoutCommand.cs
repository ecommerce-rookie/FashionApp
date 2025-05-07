using Application.Features.OrderFeatures.Models;
using Application.Messages;
using AutoMapper;
using Domain.Aggregates.OrderAggregate.Entities;
using Domain.Aggregates.OrderAggregate.Enums;
using Domain.Aggregates.OrderAggregate.ValuesObject;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;
using Domain.Shared;
using FluentValidation;
using Infrastructure.Authentication.Services;
using System.Net;

namespace Application.Features.OrderFeatures.Commands
{
    public class CheckoutCommand : ICommand<APIResponse>
    {
        public IEnumerable<CartRequestModel>? Carts { get; set; }
        public string? Address { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public string? NameReceiver { get; set; }
    }

    public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckoutCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Config();
        }

        public void Config()
        {
            RuleFor(x => x.Carts)
                .NotEmpty().WithMessage("Carts cannot be empty.")
                .NotNull().WithMessage("Carts cannot be null.")
                .ForEach(cart =>
                {
                    cart.ChildRules(c =>
                    {
                        c.RuleFor(x => x.ProductId)
                            .NotEmpty().WithMessage("Product ID cannot be empty.");

                        c.RuleFor(x => x.Quantity)
                            .NotEmpty().WithMessage("Quantity cannot be empty.")
                            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
                    });
                })
                .MustAsync(CheckExistProduct).WithMessage("Product ID doest not exist.");

            RuleFor(x => x.Address)
                .NotNull()
                .WithMessage("Address cannot be empty.");

            RuleFor(x => x.PaymentMethod)
                .NotNull()
                .WithMessage("Payment method cannot be empty.");

            RuleFor(x => x.NameReceiver)
                .NotNull()
                .WithMessage("Name receiver cannot be empty.");
        }

        private async Task<bool> CheckExistProduct(IEnumerable<CartRequestModel> carts, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ProductRepository.CheckExistProducts(carts.Select(c => c.ProductId));
        }

    }

    public class CheckoutCommandHandler : ICommandHandler<CheckoutCommand, APIResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;
        public CheckoutCommandHandler(IUnitOfWork unitOfWork, IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
        }

        public async Task<APIResponse> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var userId = _authenticationService.User.UserId;

            // Get all products
            var productIds = request.Carts!.Select(c => c.ProductId);
            var products = await _unitOfWork.ProductRepository.GetAll(p => productIds.Contains(p.Id));

            // Create order detail
            var carts = request.Carts!.Select(cart =>
            {
                var product = products.FirstOrDefault(p => p.Id.Equals(cart.ProductId));

                return Cart.Create(
                        cart.ProductId,
                        cart.Quantity,
                        product?.Price?.PurchasePrice.Amount ?? 0
                    );
            });
            // Create order
            var totalPrice = carts.Sum(c => c.Price.Amount * c.Quantity);
            var order = Order.Create(totalPrice, request.Address!, OrderStatus.Pending, (PaymentMethod)request.PaymentMethod!, request.NameReceiver!, userId);
            order.CreateOrderDetail(carts);

            // Add order to repository
            await _unitOfWork.OrderRepository.Add(order);
            await _unitOfWork.OrderRepository.AddRangeOrderDetail(order.OrderDetails!);

            // Save changes
            if(await _unitOfWork.SaveChangesAsync(cancellationToken))
            {
                return new APIResponse
                {
                    Status = HttpStatusCode.Created,
                    Message = MessageCommon.CreateSuccesfully
                };
            }

            return new APIResponse
            {
                Status = HttpStatusCode.InternalServerError,
                Message = MessageCommon.CreateFailed    
            };

        }

    }
}
