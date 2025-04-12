using Domain.SeedWorks.Events;

namespace Persistence.SeedWorks.Abstractions;

public interface IDomainEventContext
{
    IEnumerable<BaseEvent> GetDomainEvents();
}
