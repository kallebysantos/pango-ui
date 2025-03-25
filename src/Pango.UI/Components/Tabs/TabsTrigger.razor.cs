using Microsoft.AspNetCore.Components;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class TabsTrigger : ComponentBase
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
                "rounded-none px-4 pb-3 pt-2 font-semibold relative h-9 border-b-2 border-b-transparent bg-transparent text-muted-foreground shadow-none transition-none aria-[selected=true]:border-b-primary aria-[selected=true]:shadow-none",
            // Variants.Default
            _ =>
                "rounded-md px-3 font-medium transition-all disabled:pointer-events-none disabled:opacity-50 aria-[selected=true]::shadow",
        };
}
