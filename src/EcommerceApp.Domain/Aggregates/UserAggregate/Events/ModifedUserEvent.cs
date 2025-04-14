using Domain.SeedWorks.Events;

namespace Domain.Aggregates.UserAggregate.Events
{
    public class ModifedUserEvent : BaseEvent
    {
        public Guid? Id { get; set; }
        public string? Avatar { get; set; }

        public ModifedUserEvent() { }

        public ModifedUserEvent(Guid? id)
        {
            Id = id;
        }

        public ModifedUserEvent(Guid? id, string? avatar)
        {
            Id = id;
            Avatar = avatar;
        }

    }
}
