using System.ComponentModel;
using System.Reflection;

namespace StoreFront.Application.Extensions
{
    public static class EnumExtensions
    {
        public static bool IsInEnum<TEnum, TProperty>(this TProperty value)
        {
            return Enum.IsDefined(typeof(TEnum), value!);
        }

        public static T? GetEnum<T>(this string value) where T : struct
        {
            if (value == null)
            {
                return null; // Return null for null inputs
            }

            // Check if string can be parsed as integer and corresponds to a valid enum value
            if (int.TryParse(value, out int parsedInt) && Enum.IsDefined(typeof(T), parsedInt))
            {
                return (T)(object)parsedInt;
            }

            // Convert string to enum name and check if valid
            if (Enum.GetNames(typeof(T)).Contains(value, StringComparer.OrdinalIgnoreCase))
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }

            // Return null if no valid role found
            return null;
        }

        public static string GetDescription(this Enum path)
        {
            var field = path.GetType().GetField(path.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? path.ToString();
        }
    }
}
