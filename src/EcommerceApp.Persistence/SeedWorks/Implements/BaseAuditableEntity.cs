namespace Persistence.SeedWorks.Implements
{
    public class BaseAuditableEntity<T> : BaseEntity<T> where T : class
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
