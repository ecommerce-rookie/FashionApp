namespace Persistence.SeedWorks.Abstractions;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
