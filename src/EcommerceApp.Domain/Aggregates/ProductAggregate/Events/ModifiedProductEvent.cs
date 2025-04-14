using Domain.SeedWorks.Events;

namespace Domain.Aggregates.ProductAggregate.Events
{
    public class ModifiedProductEvent : BaseEvent
    {
        public Guid? Id { get; set; }

        public IEnumerable<string>? Images { get; set; }

        public ModifiedProductEvent() { }

        public ModifiedProductEvent(Guid? id)
        {
            Id = id;
        }

        public ModifiedProductEvent(Guid? id, IEnumerable<string>? Images)
        {
            Id = id;
            this.Images = Images;
        }
    }
}
