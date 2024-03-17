using System.Globalization;

namespace Pango.Extensions;

internal static class StringExtensions
{
    public static string ToKebabCase(this string value)
        => string.Concat(value.Select((letter, i) =>
            ((char.IsUpper(letter) && i > 0) ? "-" : string.Empty)
            + char.ToLowerInvariant(letter)
        ));

    public static string ToTitleCase(this string value)
        => CultureInfo.CurrentCulture.TextInfo
            .ToTitleCase(value)
            .Replace("-", string.Empty);
}