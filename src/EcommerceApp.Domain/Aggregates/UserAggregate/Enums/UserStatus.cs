using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Aggregates.UserAggregate.Enums
{
    public enum UserStatus
    {
        NotVerify = 1,
        Active = 2,
        Banned = 3,
        Deleted = 4
    }
}
