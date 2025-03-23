using Microsoft.AspNetCore.Components;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class Separator
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

    public enum Orientations
    {
        Vertical,
        Horizontal,
    }

    protected static string GetStyledOrientation(Orientations orientation) =>
        orientation switch
        {
            Orientations.Vertical => "w-[1px] h-full",
            _ => "h-[1px] w-full",
        };
}
