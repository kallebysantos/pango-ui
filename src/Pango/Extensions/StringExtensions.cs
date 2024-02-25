namespace Pango.Extensions;

internal static class StringExtensions
{
    public static string ToKebabCase(this string value)
        => string.Concat(value.Select((letter, i) =>
            ((char.IsUpper(letter) && i > 0) ? "-" : string.Empty)
            + char.ToLowerInvariant(letter)
        ));
}