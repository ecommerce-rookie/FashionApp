using Domain.Aggregates.UserAggregate.Entities;
using Domain.Aggregates.UserAggregate.Enums;
using Domain.Models.Common;
using Domain.Repositories;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class UserRepository : SqlRepository<User>, IUserRepository
    {
        public UserRepository(EcommerceContext context) : base(context)
        {
        }

        public async Task<PagedList<User>> GetUsers(int page, int eachPage, IEnumerable<UserRole>? roles, 
            IEnumerable<UserStatus>? statuss, string? search, UserRole currentRole)
        {
            var users = _dbSet
                .AsNoTracking()
                .AsSplitQuery()
                .IgnoreQueryFilters();

            if(currentRole == UserRole.Admin)
            {
                users = users.Where(x => !x.Role.Equals(UserRole.Admin.ToString()));
            }

            if(!statuss.IsNullOrEmpty())
            {
                users = users.Where(x => statuss!.Any(s => s.Equals(x.Status)));
            }

            if(!roles.IsNullOrEmpty())
            {
                users = users.Where(x => roles!.Any(s => s.Equals(x.Role)));
            }

            if(!string.IsNullOrEmpty(search))
            {
                users = users.Where(x => x.LastName.Contains(search) || x.Email.Value.Contains(search));
            }

            return await users.ToPagedListAsync(page, eachPage);
        }

    }
}
