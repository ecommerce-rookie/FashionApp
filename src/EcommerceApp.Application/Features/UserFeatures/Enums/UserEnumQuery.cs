namespace Application.Features.UserFeatures.Enums
{
    public class UserEnumQuery
    {
        public enum UserRoleQuery
        {
            Customer = 1,
            Staff = 2
        }

        public enum UserStatusQuery
        {
            NotVerify = 1,
            Active = 2,
            Banned = 3,
            Deleted = 4
        }

    }
}
