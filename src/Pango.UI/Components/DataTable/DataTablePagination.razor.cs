// SSR pagination reference: https://github.com/dotnet/aspnetcore/pull/51217

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

    [Parameter]
    public Func<int, string>? PageUrl { get; set; }

    [Parameter]
    public EventCallback? OnFirst { get; set; }

    [Parameter]
    public EventCallback? OnPrevious { get; set; }

    [Parameter]
    public EventCallback? OnNext { get; set; }

    [Parameter]
    public EventCallback? OnLast { get; set; }

    public bool CanGoBack => State.CurrentPageIndex > 0;

    public bool CanGoForwards => State.CurrentPageIndex < State.LastPageIndex;

    protected string? GetPageUrl(int? pageIndex) =>
        PageUrl is null || !pageIndex.HasValue ? null : PageUrl(pageIndex.Value);

    protected async Task GoFirstAsync()
    {
        if (!CanGoBack)
            return;
        await GoToPageAsync(0);

        if (OnFirst.HasValue)
            await OnFirst.Value.InvokeAsync();
    }

    protected async Task GoPreviousAsync()
    {
        if (!CanGoBack)
            return;

        await GoToPageAsync(State.CurrentPageIndex - 1);

        if (OnPrevious.HasValue)
            await OnPrevious.Value.InvokeAsync();
    }

    protected async Task GoNextAsync()
    {
        if (!CanGoForwards)
            return;

        await GoToPageAsync(State.CurrentPageIndex + 1);

        if (OnNext.HasValue)
            await OnNext.Value.InvokeAsync();
    }

    protected async Task GoLastAsync()
    {
        if (!CanGoForwards)
            return;

        await GoToPageAsync(State.LastPageIndex.GetValueOrDefault(0));

        if (OnLast.HasValue)
            await OnLast.Value.InvokeAsync();
    }

    protected async Task GoToPageAsync(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex > State.LastPageIndex)
            return;

        if (PageUrl is null)
        {
            await State.SetCurrentPageIndexAsync(pageIndex);
            return;
        }
    }
}
