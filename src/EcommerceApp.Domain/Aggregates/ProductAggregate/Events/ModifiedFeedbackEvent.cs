using Domain.SeedWorks.Events;

namespace Domain.Aggregates.ProductAggregate.Events
{
    public class ModifiedFeedbackEvent : BaseEvent
    {
        public Guid Id { get; set; }
    }
}
