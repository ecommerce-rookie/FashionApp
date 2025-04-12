namespace Persistence.SeedWorks.Implements
{
    public class BaseAuditableEntity<T> : BaseEntity<T>
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid Version { get; set; } = Guid.NewGuid();
    }
}
