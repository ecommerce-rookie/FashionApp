using Newtonsoft.Json;

namespace StoreFront.Application.Extensions
{
    public static class CookieExtensions
    {
        private static CookieOptions options = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true,
            IsEssential = true
        };

        public static void SetCookie<T>(this HttpResponse response, string key, T value)
        {
            var jsonValue = JsonConvert.SerializeObject(value);

            response.Cookies.Append(key, jsonValue, options);
        }

        public static T? GetCookie<T>(this HttpRequest request, string key)
        {
            if (request.Cookies.TryGetValue(key, out var value))
            {
                return JsonConvert.DeserializeObject<T>(value);
            }

            return default;
        }

        public static void DeleteCookie(this HttpResponse response, string key)
        {
            response.Cookies.Delete(key);
        }
    }
}
