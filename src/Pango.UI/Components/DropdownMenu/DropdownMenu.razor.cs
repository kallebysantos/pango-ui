using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class DropdownMenu
{
    internal ElementReference DropdownTriggerRef;

    internal ElementReference DropdownContentRef;

    private IJSObjectReference? jsDropdownRender;

    private IJSObjectReference? jsDropdownCleaner;

    private DotNetObjectReference<DropdownMenu>? csDropdownMenuRef;

    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    [Inject] protected TwMerge TwMerge { get; set; } = null!;

    /// <summary>
    /// Merge Tailwind CSS classes without style conflicts
    /// </summary>
    public string? Tw(params string?[] classNames)
    {
        return TwMerge.Merge(classNames);
    }

    private static Dictionary<Placements, string> _placements = new()
    {
        { Placements.BottomStart, "bottom-start" },
        { Placements.TopStart, "top-start" },
        { Placements.LeftStart, "left-start" },
        { Placements.RightStart, "right-start" },

        { Placements.BottomCenter, "bottom-center" },
        { Placements.TopCenter, "top-center" },
        { Placements.LeftCenter, "left-center" },
        { Placements.RightCenter, "right-center" },

        { Placements.BottomEnd, "bottom-end" },
        { Placements.TopEnd, "top-end" },
        { Placements.LeftEnd, "left-end" },
        { Placements.RightEnd, "right-end" },
    };

    public enum Placements
    {
        BottomStart,
        TopStart,
        LeftStart,
        RightStart,

        BottomCenter,
        TopCenter,
        LeftCenter,
        RightCenter,

        BottomEnd,
        TopEnd,
        LeftEnd,
        RightEnd,
    }
}
