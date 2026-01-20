using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class Select<TValue> : InputBase<TValue>, ISelect
{
    [Inject] protected TwMerge TwMerge { get; set; } = null!;

    /// <summary>
    /// Merge Tailwind CSS classes without style conflicts
    /// </summary>
    private string? Tw(params string?[] classNames) => TwMerge.Merge(classNames);

    private IJSObjectReference? _jsDropdownRender;
    private IJSObjectReference? _jsDropdownCleaner;
    private DotNetObjectReference<Select<TValue>>? _csSelectRef;

    public List<SelectItem> AllItems { get; } = [];
    public int FocusedIndex { get; set; }
    public ElementReference? SelectTriggerRef { get; set; }
    public ElementReference? SelectContentRef { get; set; }
    public object? SelectValue { get; set; }
    public SelectItem? SelectedItem { get; set; }
    public List<SelectItem> SelectedItems { get; set; } = [];
    public bool Open { get; set; }

    public string[] SelectKeys { get; } = [" ", "Enter"];

    protected override bool TryParseValueFromString(
        string? value,
        out TValue result,
        out string validationErrorMessage)
    {
        if (BindConverter.TryConvertTo<TValue>(
                value,
                CultureInfo.CurrentCulture,
                out var parsedValue))
        {
            result = parsedValue;
            validationErrorMessage = string.Empty;
            return true;
        }

        result = default!;
        validationErrorMessage = "invalid-value";
        return false;
    }

    public void RegisterItem(SelectItem item)
    {
        if (!AllItems.Contains(item))
        {
            AllItems.Add(item);
        }
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
