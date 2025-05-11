namespace Infrastructure.Shared.Extensions
{
    public static class AvatarExtensions
    {
        public static string GetAvatarV1(this string fullName)
        {
            return $"https://ui-avatars.com/api/?name={fullName}&background=random&rounded=true&color=random";
        }

        public static string GetAvatar(this string fullName)
        {
            return $"https://avatar.iran.liara.run/username?username={fullName}";
        }
    }
}
