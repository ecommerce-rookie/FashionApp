namespace Persistence.SeedWorks.Abstractions;

public interface ITrackableEntity
{
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}
