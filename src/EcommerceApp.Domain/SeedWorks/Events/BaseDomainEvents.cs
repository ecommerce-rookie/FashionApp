using Domain.SeedWorks.Abstractions;
using Persistence.SeedWorks.Implements;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.SeedWorks.Events;

public abstract class BaseDomainEvents<T> : BaseAuditableEntity<T>, IBaseDomainEvent
{
    private readonly List<BaseEvent> _domainEvents = [];

    [NotMapped] public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void RegisterDomainEvent(BaseEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void RemoveDomainEvent(BaseEvent domainEvent) => _domainEvents.Remove(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}