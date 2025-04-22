using Application.Features.UserFeatures.Models;
using Newtonsoft.Json;
using StoreFront.Domain.Constants;

namespace StoreFront.Application.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static void SignIn(this ISession session, AuthorResponse user, int numberOfProduct, bool isAdmin = false)
        {
            session.SetObjectAsJson(SessionConstant.UserSession, user);
            session.SetObjectAsJson(SessionConstant.IsAdmin, isAdmin);
            session.SetObjectAsJson(SessionConstant.CartSession, numberOfProduct);
        }

        public static bool IsAuthenticated(this ISession session)
        {
            return session.GetObjectFromJson<AuthorResponse>(SessionConstant.UserSession) != null;
        }

        public static void SignOut(this ISession session)
        {
            session.Clear();
        }

        public static T GetCurrentUser<T>(this ISession session)
        {
            return session.GetObjectFromJson<T>(SessionConstant.UserSession)!;
        }

        public static bool IsAdmin(this ISession session)
        {
            return session.GetObjectFromJson<bool>(SessionConstant.IsAdmin);
        }

        public static int GetNumberOfProduct(this ISession session)
        {
            return session.GetObjectFromJson<int>(SessionConstant.CartSession);
        }

        public static void UpdateNumberOfProduct(this ISession session, int numberOfProduct)
        {
            session.SetObjectAsJson(SessionConstant.CartSession, numberOfProduct);
        }
    }
}
