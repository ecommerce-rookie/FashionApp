using Domain.Aggregates.UserAggregate.Entities;
using Domain.Repositories.BaseRepositories;

namespace Domain.Repositories
{
    public interface IUserRepository : ISqlRepository<User>
    {
    }
}
