using Domain.SeedWorks.Events;

namespace Domain.Aggregates.FeedbackAggregate.Events
{
    public class ModifiedFeedbackEvent : BaseEvent
    {
        public Guid Id { get; set; }
    }
}
