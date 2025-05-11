using Domain.Aggregates.FeedbackAggregate.Entities;
using Domain.Exceptions;

namespace Application.UnitTest.Domain.AggregateTest
{
    public class FeedbackAggregateTest
    {
        [Fact]
        public void Create_ValidParameters_ShouldReturnFeedback()
        {
            // Arrange
            var content = "Great product!";
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var rating = 5;

            // Act
            var feedback = Feedback.Create(content, userId, productId, rating);

            // Assert
            Assert.NotNull(feedback);
            Assert.Equal(content, feedback.Content);
            Assert.Equal(userId, feedback.UserId);
            Assert.Equal(productId, feedback.ProductId);
            Assert.Equal(rating, feedback.Rating);
            Assert.False(feedback.IsDeleted);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Create_InvalidContent_ShouldThrowValidationException(string content)
        {
            // Arrange
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var rating = 5;

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => Feedback.Create(content, userId, productId, rating));
            Assert.Equal("One or more validation failures have occurred.", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void Create_InvalidRating_ShouldThrowValidationException(int rating)
        {
            // Arrange
            var content = "Great product!";
            var userId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => Feedback.Create(content, userId, productId, rating));
            Assert.Equal("One or more validation failures have occurred.", exception.Message);
        }

        [Fact]
        public void Update_ValidParameters_ShouldUpdateFeedback()
        {
            // Arrange
            var feedback = new Feedback(Guid.NewGuid(), "Initial content", Guid.NewGuid(), Guid.NewGuid(), 3);
            var newContent = "Updated content";
            var newRating = 4;

            // Act
            feedback.Update(newContent, newRating);

            // Assert
            Assert.Equal(newContent, feedback.Content);
            Assert.Equal(newRating, feedback.Rating);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Update_InvalidContent_ShouldThrowValidationException(string content)
        {
            // Arrange
            var feedback = new Feedback(Guid.NewGuid(), "Initial content", Guid.NewGuid(), Guid.NewGuid(), 3);
            var rating = 4;

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => feedback.Update(content, rating));
            Assert.Equal("One or more validation failures have occurred.", exception.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void Update_InvalidRating_ShouldThrowValidationException(int rating)
        {
            // Arrange
            var feedback = new Feedback(Guid.NewGuid(), "Initial content", Guid.NewGuid(), Guid.NewGuid(), 3);
            var content = "Updated content";

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => feedback.Update(content, rating));
            Assert.Equal("One or more validation failures have occurred.", exception.Message);
        }

        [Fact]
        public void Delete_ShouldSetIsDeletedToTrue()
        {
            // Arrange
            var feedback = new Feedback(Guid.NewGuid(), "Content", Guid.NewGuid(), Guid.NewGuid(), 5);

            // Act
            feedback.Delete();

            // Assert
            Assert.True(feedback.IsDeleted);
        }
    }
}
