using Microsoft.AspNetCore.Components;
using TailwindMerge;

namespace Pango.UI.Components;

public enum DialogState
{
    Default,
    Open,
    Closed
}

public partial class Dialog : ComponentBase
{
    /// <summary>
    ///     Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    [Inject]
    protected TwMerge TwMerge { get; set; } = null!;

    /// <summary>
    /// Merge Tailwind CSS classes without style conflicts
    /// </summary>
    public string? Tw(params string?[] classNames) => TwMerge.Merge(classNames);

    [Parameter]
    public DialogState State { get; set; } = DialogState.Default;

    protected string GetDialogStateName() =>
        State switch
        {
            DialogState.Open => "open",
            DialogState.Closed => "closed",
            _ => string.Empty
        };
}
