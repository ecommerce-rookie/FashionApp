using Infrastructure.Extensions;

namespace Application.UnitTest.Persistance.Extensions
{
    public class QueryableExtensionsTest
    {
        private class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private IQueryable<TestEntity> CreateTestData()
        {
            return new List<TestEntity>
        {
            new() { Id = 1, Name = "B" },
            new() { Id = 2, Name = "A" },
            new() { Id = 3, Name = "C" }
        }.AsQueryable();
        }

        [Theory]
        [InlineData("Id", true)]
        [InlineData("Id", false)]
        [InlineData("Name", true)]
        [InlineData("Name", false)]
        public void Sort_SortsCorrectly(string property, bool isAscending)
        {
            var data = CreateTestData();

            var result = data.Sort(property, isAscending).ToList();

            Assert.Equal(3, result.Count);
            if (isAscending)
            {
                Assert.True(result.First().GetType().GetProperty(property)!.GetValue(result.First())!
                    .ToString()!.CompareTo(result.Last().GetType().GetProperty(property)!.GetValue(result.Last())!.ToString()) <= 0);
            } else
            {
                Assert.True(result.First().GetType().GetProperty(property)!.GetValue(result.First())!
                    .ToString()!.CompareTo(result.Last().GetType().GetProperty(property)!.GetValue(result.Last())!.ToString()) >= 0);
            }
        }

        [Fact]
        public void Sort_ThrowsException_WhenPropertyNameInvalid()
        {
            var data = CreateTestData();
            Assert.Throws<ArgumentException>(() => data.Sort("", true));
        }

        [Fact]
        public void PaginateAndSort_WorksCorrectly()
        {
            var data = CreateTestData();
            var result = data.PaginateAndSort(1, 2, "Name", true).ToList();

            Assert.Equal(2, result.Count);
            Assert.Equal("A", result[0].Name);
            Assert.Equal("B", result[1].Name);
        }

        [Fact]
        public void ToPaginateAndSort_ReturnsPagedList()
        {
            var data = CreateTestData();
            var result = data.ToPaginateAndSort(1, 2, "Name", true);

            Assert.Equal(3, result.TotalItems);
            Assert.Equal(2, result.TotalPages);
            Assert.Equal(2, result.EachPage);
            Assert.Equal(1, result.CurrentPage);
        }

        [Fact]
        public void Pagination_WorksCorrectly()
        {
            var data = CreateTestData();
            var result = data.Pagination(2, 2).ToList();

            Assert.Single(result);
            Assert.Equal("C", result[0].Name);
        }
    }
}
