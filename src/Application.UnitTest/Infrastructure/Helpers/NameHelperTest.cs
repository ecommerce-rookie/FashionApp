using FluentAssertions;
using Infrastructure.Shared.Helpers;

namespace Application.UnitTest.Infrastructure.Helpers
{
    public class NameHelperTest
    {
        [Fact]
        public void GenerateName_WithNoArguments_ShouldReturnHandlerName()
        {
            // Arrange
            var handler = new TestHandler();
            object[] arguments = new object[0];

            // Act
            var result = NameHelper.GenerateName(handler, arguments);

            // Assert
            result.Should().Be("TestHandler");
        }

        [Fact]
        public void GenerateName_WithObjectWithProperties_ShouldReturnHandlerNameWithPropertyValues()
        {
            // Arrange
            var handler = new TestHandler();
            var testObject = new TestObject { Name = "Test", Age = 30 };
            object[] arguments = new object[] { testObject };

            // Act
            var result = NameHelper.GenerateName(handler, arguments);

            // Assert
            result.Should().Be("TestHandler_Test_30");
        }

        [Fact]
        public void GenerateName_WithObjectWithoutProperties_ShouldReturnHandlerNameWithToString()
        {
            // Arrange
            var handler = new TestHandler();
            var testObject = 42; // Integer without properties
            object[] arguments = new object[] { testObject };

            // Act
            var result = NameHelper.GenerateName(handler, arguments);

            // Assert
            result.Should().Be("TestHandler_42");
        }

        [Fact]
        public void GenerateName_WithNullArgument_ShouldReturnHandlerNameWithNull()
        {
            // Arrange
            var handler = new TestHandler();
            object[] arguments = new object[] { };

            // Act
            var result = NameHelper.GenerateName(handler, arguments);

            // Assert
            result.Should().Be("TestHandler");
        }

        [Fact]
        public void GenerateName_WithObjectWithNullProperties_ShouldReturnHandlerNameWithNullValues()
        {
            // Arrange
            var handler = new TestHandler();
            var testObject = new TestObject { Name = null, Age = null };
            object[] arguments = new object[] { testObject };

            // Act
            var result = NameHelper.GenerateName(handler, arguments);

            // Assert
            result.Should().Be("TestHandler_null_null");
        }

        private class TestHandler { }

        private class TestObject
        {
            public string Name { get; set; }
            public int? Age { get; set; }
        }

        private class EmptyObject { }

    }

}
