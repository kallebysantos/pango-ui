using System.Globalization;
using System.Text;

namespace Pango.UI.Shared;

public static class StringExtensions
{
    public static string ToKebabCase(this string value) =>
        string.Concat(
            value.Select(
                (letter, i) =>
                    ((char.IsUpper(letter) && i > 0) ? "-" : string.Empty)
                    + char.ToLowerInvariant(letter)
            )
        );

    public static string ToTitleCase(this string value) =>
        CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value).Replace("-", string.Empty);

    public static string JoinMerge(this IEnumerable<string> values, char separator) =>
        values
            .Aggregate<string, (StringBuilder, string)>(
                seed: (new StringBuilder(), string.Empty),
                func: (state, fragment) =>
                    state.Item2 != fragment
                        ? (state.Item1.AppendWithSeparator(separator, fragment), fragment)
                        : state
            )
            .Item1.ToString();

    public static StringBuilder AppendWithSeparator(
        this StringBuilder builder,
        char separator,
        string value
    )
    {
        if (string.IsNullOrEmpty(value))
            return builder;

        return builder.Length == 0 ? builder.Append(value) : builder.Append(separator + value);
    }
}
