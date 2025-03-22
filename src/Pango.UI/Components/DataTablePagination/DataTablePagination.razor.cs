using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class DataTablePagination : Paginator
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

    protected Task GoFirstAsync() => GoToPageAsync(0);

    protected Task GoPreviousAsync() => GoToPageAsync(State.CurrentPageIndex - 1);

    protected Task GoNextAsync() => GoToPageAsync(State.CurrentPageIndex + 1);

    protected Task GoLastAsync() => GoToPageAsync(State.LastPageIndex.GetValueOrDefault(0));

    protected bool CanGoBack => State.CurrentPageIndex > 0;
    protected bool CanGoForwards => State.CurrentPageIndex < State.LastPageIndex;

    private Task GoToPageAsync(int pageIndex) => State.SetCurrentPageIndexAsync(pageIndex);
}
