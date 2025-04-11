using Persistence.SeedWorks.Abstractions;

namespace Persistence.SeedWorks.Implements
{
    public abstract class BaseEntity<T> : IEntity<T> where T : class
    {
        public required T Id { get; set; }
    }
}
