using Application.Features.OrderFeatures.Commands;
using Application.Features.OrderFeatures.Models;
using Domain.Aggregates.OrderAggregate.Enums;
using Domain.Repositories.BaseRepositories;
using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;

namespace Application.UnitTest.Application.Features.OrderTest.CheckoutTest
{
    public class CheckoutCommandValidatorTest
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CheckoutCommandValidator _validator;

        public CheckoutCommandValidatorTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUnitOfWork.Setup(u => u.ProductRepository.CheckExistProducts(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(true);

            _validator = new CheckoutCommandValidator(_mockUnitOfWork.Object);
        }

        private CheckoutCommand CreateValidCommand()
        {
            return new CheckoutCommand
            {
                Carts = new List<CartRequestModel>
                {
                    new CartRequestModel { ProductId = Guid.NewGuid(), Quantity = 1 }
                },
                Address = "123 Street",
                PaymentMethod = PaymentMethod.Cash,
                NameReceiver = "John Doe"
            };
        }

        [Fact]
        public async Task Should_Pass_When_Valid()
        {
            var command = CreateValidCommand();

            var result = await _validator.ValidateAsync(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Should_Fail_When_Carts_Null()
        {
            var command = CreateValidCommand();
            command.Carts = null;

            var result = await _validator.TestValidateAsync(command);

            result.Errors.Should().Contain(e => e.PropertyName == "Carts");
        }

        [Fact]
        public async Task Should_Fail_When_Carts_Empty()
        {
            var command = CreateValidCommand();
            command.Carts = new List<CartRequestModel>();

            var result = await _validator.ValidateAsync(command);

            result.Errors.Should().Contain(e => e.PropertyName == "Carts");
        }

        [Fact]
        public async Task Should_Fail_When_ProductId_Empty()
        {
            var command = CreateValidCommand();
            command.Carts = new List<CartRequestModel> { new CartRequestModel { ProductId = Guid.Empty, Quantity = 1 } };

            var result = await _validator.ValidateAsync(command);

            result.Errors.Should().Contain(e => e.PropertyName.Contains("ProductId"));
        }

        [Fact]
        public async Task Should_Fail_When_Quantity_Invalid()
        {
            var command = CreateValidCommand();
            command.Carts = new List<CartRequestModel> { new CartRequestModel { ProductId = Guid.NewGuid(), Quantity = 0 } };

            var result = await _validator.ValidateAsync(command);

            result.Errors.Should().Contain(e => e.PropertyName.Contains("Quantity"));
        }

        [Fact]
        public async Task Should_Fail_When_ProductId_Not_Exist()
        {
            _mockUnitOfWork.Setup(u => u.ProductRepository.CheckExistProducts(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(false);

            var command = CreateValidCommand();

            var result = await _validator.ValidateAsync(command);

            result.Errors.Should().Contain(e => e.ErrorMessage.Contains("doest not exist"));
        }

        [Fact]
        public async Task Should_Fail_When_Address_Null()
        {
            var command = CreateValidCommand();
            command.Address = null;

            var result = await _validator.ValidateAsync(command);

            result.Errors.Should().Contain(e => e.PropertyName == "Address");
        }

        [Fact]
        public async Task Should_Fail_When_PaymentMethod_And_NameReceiver_Null()
        {
            var command = CreateValidCommand();
            command.PaymentMethod = null;
            command.NameReceiver = null;

            var result = await _validator.ValidateAsync(command);

            result.Errors.Should().Contain(e => e.PropertyName == "PaymentMethod");
            result.Errors.Should().Contain(e => e.PropertyName == "NameReceiver");
        }
    }
}
