using Domain.SeedWorks.Events;

namespace Domain.Aggregates.ProductAggregate.Events
{
    public class ModifiedCategoryEvent : BaseEvent
    {
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public ModifiedCategoryEvent() { }

        public ModifiedCategoryEvent(string categoryName)
        {
            CategoryName = categoryName;
        }

        public ModifiedCategoryEvent(int? categoryId, string? categoryName)
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
        }
    }
}
