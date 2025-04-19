using System.Globalization;
using System.Text;

public static class SlugHelper
{
    /// <summary>
    /// Generates a URL-friendly slug from the given input string.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Generate(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Normalize the string to decompose characters with diacritics
        var normalized = input.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(capacity: normalized.Length);

        // Iterate through each character in the normalized string
        bool wasDash = false;
        foreach (var c in normalized)
        {
            // Skip non-spacing marks (diacritics)
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;

            // Convert to lowercase and check if it's a valid character
            var lc = char.ToLowerInvariant(c);

            // Check if the character is alphanumeric or a dash
            if ((lc >= 'a' && lc <= 'z') || (lc >= '0' && lc <= '9'))
            {
                // Append the character to the StringBuilder
                sb.Append(lc);
                wasDash = false;
            } else
            {
                // If the character is not alphanumeric, replace it with a dash
                if (!wasDash)
                {
                    sb.Append('-');
                    wasDash = true;
                }
            }
        }

        // Remove any trailing dashes
        return sb
            .ToString()
            .Trim('-');
    }
}
