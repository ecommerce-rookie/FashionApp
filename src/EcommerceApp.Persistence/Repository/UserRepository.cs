using Domain.Aggregates.UserAggregate.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repository
{
    public class UserRepository : SqlRepository<User>, IUserRepository
    {
        public UserRepository(EcommerceContext context) : base(context)
        {
        }
    }
}
