using Infrastructure.Shared.Extensions;
using System.ComponentModel;

namespace Application.UnitTest.Infrastructure.Extensions
{
    public class EnumExtensionTest
    {
        public class EnumExtensionsTests
        {
            public enum SampleEnum
            {
                [Description("First Option")]
                Option1 = 1,

                [Description("Second Option")]
                Option2 = 2,

                Option3 = 3
            }

            [Fact]
            public void IsInEnum_Should_ReturnTrue_For_ValidEnumValue()
            {
                // Arrange
                var value = 1;

                // Act
                var result = value.IsInEnum<SampleEnum, int>();

                // Assert
                Assert.True(result);
            }

            [Fact]
            public void IsInEnum_Should_ReturnFalse_For_InvalidEnumValue()
            {
                // Arrange
                var value = 999;

                // Act
                var result = value.IsInEnum<SampleEnum, int>();

                // Assert
                Assert.False(result);
            }

            [Fact]
            public void GetEnum_Should_ReturnEnum_For_ValidStringValue()
            {
                // Arrange
                var value = "Option1";

                // Act
                var result = value.GetEnum<SampleEnum>();

                // Assert
                Assert.Equal(SampleEnum.Option1, result);
            }

            [Fact]
            public void GetEnum_Should_ReturnNull_For_InvalidStringValue()
            {
                // Arrange
                var value = "InvalidOption";

                // Act
                var result = value.GetEnum<SampleEnum>();

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void GetEnum_Should_ReturnEnum_For_ValidIntegerValue()
            {
                // Arrange
                var value = "1";

                // Act
                var result = value.GetEnum<SampleEnum>();

                // Assert
                Assert.Equal(SampleEnum.Option1, result);
            }

            [Fact]
            public void GetEnum_Should_ReturnNull_For_InvalidIntegerValue()
            {
                // Arrange
                var value = "999";

                // Act
                var result = value.GetEnum<SampleEnum>();

                // Assert
                Assert.Null(result);
            }

            [Fact]
            public void GetDescription_Should_ReturnDescription_ForEnumWithDescription()
            {
                // Arrange
                var value = SampleEnum.Option1;

                // Act
                var result = value.GetDescription();

                // Assert
                Assert.Equal("First Option", result);
            }

            [Fact]
            public void GetDescription_Should_ReturnEnumName_WhenNoDescription()
            {
                // Arrange
                var value = SampleEnum.Option3;

                // Act
                var result = value.GetDescription();

                // Assert
                Assert.Equal("Option3", result);
            }
        }

    }
}
