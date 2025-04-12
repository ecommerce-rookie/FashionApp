using MediatR;

namespace Domain.SeedWorks.Events;

public abstract class BaseEvent : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}