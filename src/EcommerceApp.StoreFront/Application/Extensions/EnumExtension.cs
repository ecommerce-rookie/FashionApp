using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.Reflection;

namespace StoreFront.Application.Extensions
{
    public static class EnumExtension
    {
        public static IEnumerable<SelectListItem> ToSelectList<TEnum>(bool useDescription = false) where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new SelectListItem
                {
                    Value = Convert.ToInt32(e).ToString(),
                    Text = useDescription ? GetDescription(e) : e.ToString()
                })
                .ToList();
        }

        private static string GetDescription<TEnum>(TEnum enumValue) where TEnum : Enum
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? enumValue.ToString();
        }
    }
}
