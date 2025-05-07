using Microsoft.AspNetCore.Components;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class Alert
{
    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    [Inject]
    protected TwMerge TwMerge { get; set; } = null!;

    /// <summary>
    /// Merge Tailwind CSS classes without style conflicts
    /// </summary>
    public string? Tw(params string?[] classNames) => TwMerge.Merge(classNames);

    public enum Variants
    {
        Default,
        Sucess,
        Destructive,
    }

    protected static string GetStyledVariant(Variants variant) =>
        variant switch
        {
            Variants.Sucess =>
                "border-emerald-500/50 text-emerald-500 dark:border-emerald-500 [&>i]:text-emerald-500",
            Variants.Destructive =>
                "border-destructive/50 text-destructive dark:border-destructive [&>i]:text-destructive",
            // Variants.Default
            _ => "text-foreground [&>i]:text-foreground",
        };
}
