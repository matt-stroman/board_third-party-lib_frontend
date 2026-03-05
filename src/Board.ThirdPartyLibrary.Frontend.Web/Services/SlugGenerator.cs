using System.Globalization;
using System.Text;

namespace Board.ThirdPartyLibrary.Frontend.Web.Services;

/// <summary>
/// Generates normalized kebab-case slugs from display strings.
/// </summary>
internal static class SlugGenerator
{
    /// <summary>
    /// Converts arbitrary text into a lowercase kebab-case slug.
    /// </summary>
    /// <param name="value">Source display string.</param>
    /// <param name="fallback">Fallback slug when no usable characters exist.</param>
    /// <returns>Normalized kebab-case slug.</returns>
    public static string ToKebabCase(string? value, string fallback = "item")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        var previousWasDash = false;

        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(character))
            {
                builder.Append(char.ToLowerInvariant(character));
                previousWasDash = false;
                continue;
            }

            if (builder.Length == 0 || previousWasDash)
            {
                continue;
            }

            builder.Append('-');
            previousWasDash = true;
        }

        var slug = builder.ToString().Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? fallback : slug;
    }
}
