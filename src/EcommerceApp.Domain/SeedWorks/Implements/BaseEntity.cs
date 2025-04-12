using Domain.SeedWorks.Events;
using Persistence.SeedWorks.Abstractions;

namespace Persistence.SeedWorks.Implements
{
    public abstract class BaseEntity<T> : IEntity<T>
    {
        public T Id { get; set; } = default!;
    }
}
