using Domain.SeedWorks.Events;

namespace Domain.SeedWorks.Abstractions
{
    public interface IBaseDomainEvent
    {
        IReadOnlyCollection<BaseEvent> DomainEvents { get; }
        void ClearDomainEvents();

    }
}
