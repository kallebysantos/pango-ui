using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class Checkbox : InputCheckbox, IAsyncDisposable
{
    [Inject]
    private IJSRuntime? JS { get; set; }

    [Inject]
    protected TwMerge TwMerge { get; set; } = null!;

    /// <summary>
    /// Merge Tailwind CSS classes without style conflicts
    /// </summary>
    private string? Tw(params string?[] classNames) => TwMerge.Merge(classNames);
    private IJSObjectReference? _jsModule;
    private bool _disposed;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (JS is null) return;

        _jsModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "./Components/Checkbox/Checkbox.razor.js");

        if (_updateIndeterminate)
        {
            await _jsModule.InvokeVoidAsync("setIndeterminate", Element, Indeterminate);
            _updateIndeterminate = false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        if (_jsModule is not null)
        {
            await _jsModule.DisposeAsync();
        }
    }
}

