using Application.Behaviors;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;

namespace Application.UnitTest.Application.PipelineTest
{
    public class ValidationBehaviourTest
    {
        private readonly Mock<IValidator<SampleRequest>> _validatorMock = new();

        public class SampleRequest { }

        public class SampleResponse { }

        [Fact]
        public async Task Handle_Should_Invoke_Next_When_No_Validators()
        {
            // Arrange
            var request = new SampleRequest();
            var response = new SampleResponse();
            var next = new RequestHandlerDelegate<SampleResponse>((ctx) => Task.FromResult(response));
            var behaviour = new ValidationBehaviour<SampleRequest, SampleResponse>(Enumerable.Empty<IValidator<SampleRequest>>());

            // Act
            var result = await behaviour.Handle(request, next, default);

            // Assert
            result.Should().Be(response);
        }

        [Fact]
        public async Task Handle_Should_Invoke_Next_When_Validators_Have_No_Errors()
        {
            // Arrange
            var request = new SampleRequest();
            var response = new SampleResponse();
            var next = new RequestHandlerDelegate<SampleResponse>((ctx) => Task.FromResult(response));

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var validators = new List<IValidator<SampleRequest>> { _validatorMock.Object };
            var behaviour = new ValidationBehaviour<SampleRequest, SampleResponse>(validators);

            // Act
            var result = await behaviour.Handle(request, next, default);

            // Assert
            result.Should().Be(response);
            _validatorMock.Verify(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Throw_When_Validation_Errors_Occur()
        {
            // Arrange
            var request = new SampleRequest();
            var next = new RequestHandlerDelegate<SampleResponse>((ctx) => Task.FromResult(new SampleResponse()));

            var failures = new List<ValidationFailure> { new ValidationFailure("Prop", "Error") };
            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<SampleRequest>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(failures));

            var validators = new List<IValidator<SampleRequest>> { _validatorMock.Object };
            var behaviour = new ValidationBehaviour<SampleRequest, SampleResponse>(validators);

            // Act & Assert
            var act = async () => await behaviour.Handle(request, next, default);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage("*Prop*")
                .Where(ex => ex.Errors.Any(e => e.PropertyName == "Prop" && e.ErrorMessage == "Error"));
        }
    }
}
