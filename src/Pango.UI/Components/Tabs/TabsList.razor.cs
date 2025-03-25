using Microsoft.AspNetCore.Components;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class TabsList : ComponentBase
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

    protected static string GetStyledVariant(Tabs.Variants variant) =>
        variant switch
        {
            Tabs.Variants.Underline =>
                "w-full justify-start rounded-none border-b bg-transparent p-0",
            // Variants.Default
            _ => "rounded-lg bg-muted p-1 justify-center",
        };
}
