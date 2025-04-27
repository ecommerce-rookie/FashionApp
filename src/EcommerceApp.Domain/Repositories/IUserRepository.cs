using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Models.Common;
using Domain.Repositories.BaseRepositories;

namespace Domain.Repositories
{
    public interface IUserRepository : ISqlRepository<User>
    {
        Task<PagedList<User>> GetUsers(int page, int eachPage, IEnumerable<UserRole>? roles, 
            IEnumerable<UserStatus>? statuss, string? search, UserRole currentRole);
    }
}
