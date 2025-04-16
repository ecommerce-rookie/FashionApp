using System.Text.Json;

namespace StoreFront.Application.Helpers
{
    public class JsonHelper
    {
        public static JsonNamingPolicy GetJsonNamingPolicy(string naming)
        {
            return naming switch
            {
                "CamelCase" => JsonNamingPolicy.CamelCase,
                "KebabCaseLower" => JsonNamingPolicy.KebabCaseLower,
                "KebabCaseUpper" => JsonNamingPolicy.KebabCaseUpper,
                "SnakeCaseLower" => JsonNamingPolicy.SnakeCaseLower,
                "SnakeCaseUpper" => JsonNamingPolicy.SnakeCaseUpper,
                _ => throw new ArgumentException("Invalid naming policy")
            };
        }
    }
}
