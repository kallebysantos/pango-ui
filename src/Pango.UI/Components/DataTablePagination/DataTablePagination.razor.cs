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
    protected string? Tw(params string?[] classNames) => TwMerge.Merge(classNames);

    public Task GoFirstAsync() => GoToPageAsync(0);

    public Task GoPreviousAsync() => GoToPageAsync(State.CurrentPageIndex - 1);

    public Task GoNextAsync() => GoToPageAsync(State.CurrentPageIndex + 1);

    public Task GoLastAsync() => GoToPageAsync(State.LastPageIndex.GetValueOrDefault(0));

    public bool CanGoBack => State.CurrentPageIndex > 0;

    public bool CanGoForwards => State.CurrentPageIndex < State.LastPageIndex;

    public Task GoToPageAsync(int pageIndex) => State.SetCurrentPageIndexAsync(pageIndex);
}
