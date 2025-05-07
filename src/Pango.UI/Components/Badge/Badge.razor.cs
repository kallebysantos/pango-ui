using Microsoft.AspNetCore.Components;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class Badge
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
        Secondary,
        Destructive,
        Outline,
    }

    protected static string GetStyledVariant(Variants variant) =>
        variant switch
        {
            Variants.Secondary =>
                "border-transparent bg-secondary text-secondary-foreground hover:bg-secondary/80",
            Variants.Outline => "text-foreground",
            Variants.Destructive =>
                "border-transparent bg-destructive text-destructive-foreground shadow hover:bg-destructive/80",
            // Variants.Default
            _ => "border-transparent bg-primary text-primary-foreground shadow hover:bg-primary/80",
        };
}
