using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class Select<TValue> : InputBase<TValue>, ISelect, IDisposable
{
    public ElementReference SelectTriggerRef { get; set; }
    public ElementReference SelectContentRef { get; set; }

    private IJSObjectReference? _jsSelectModule;
    private IJSObjectReference? _jsSelectCleaner;
    private DotNetObjectReference<Select<TValue>>? _csSelectRef;

    private readonly List<SelectItem<TValue>> _items = [];

    [Inject]
    protected IJSRuntime JS { get; set; } = null!;

    [Inject]
    protected TwMerge TwMerge { get; set; } = null!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public string? Placeholder { get; set; }

    [Parameter]
    public Placements Placement { get; set; } = Placements.BottomStart;

    [Parameter]
    public bool Disabled { get; set; }


    public string State { get; private set; } = "closed";

    public string? SelectedText { get; private set; }

    /// <summary>
    /// Exposes the current value for child components
    /// </summary>
    internal TValue? SelectedValue => CurrentValue;

    public string? Tw(params string?[] classNames) => TwMerge.Merge(classNames);

    private static readonly Dictionary<Placements, string> _placements = new()
    {
        { Placements.BottomStart, "bottom-start" },
        { Placements.TopStart, "top-start" },
        { Placements.LeftStart, "left-start" },
        { Placements.RightStart, "right-start" },
        { Placements.BottomCenter, "bottom" },
        { Placements.TopCenter, "top" },
        { Placements.LeftCenter, "left" },
        { Placements.RightCenter, "right" },
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

    protected override void OnInitialized()
    {
        _csSelectRef = DotNetObjectReference.Create(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsSelectModule = await JS.InvokeAsync<IJSObjectReference>(
                "import",
                "./Components/Select/Select.razor.js"
            );
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override bool TryParseValueFromString(
        string? value,
        [MaybeNullWhen(false)] out TValue result,
        [NotNullWhen(false)] out string? validationErrorMessage)
    {
        var targetType = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

        if (targetType == typeof(string))
        {
            result = (TValue)(object)(value ?? string.Empty);
            validationErrorMessage = null;
            return true;
        }

        if (string.IsNullOrEmpty(value))
        {
            result = default!;
            validationErrorMessage = null;
            return true;
        }

        if (targetType.IsEnum)
        {
            if (Enum.TryParse(targetType, value, ignoreCase: true, out var enumResult))
            {
                result = (TValue)enumResult!;
                validationErrorMessage = null;
                return true;
            }
        }

        try
        {
            result = (TValue)Convert.ChangeType(value, targetType)!;
            validationErrorMessage = null;
            return true;
        }
        catch
        {
            result = default!;
            validationErrorMessage = $"The value '{value}' is not valid for {typeof(TValue).Name}.";
            return false;
        }
    }

    internal void RegisterItem(SelectItem<TValue> item)
    {
        if (!_items.Contains(item))
        {
            _items.Add(item);

            // Set initial selected text if value matches
            if (EqualityComparer<TValue>.Default.Equals(item.Value, SelectedValue))
            {
                SelectedText = item.GetDisplayText();
                StateHasChanged();
            }
        }
    }

    internal void UnregisterItem(SelectItem<TValue> item)
    {
        _items.Remove(item);
    }

    internal async Task SelectItemAsync(TValue value, string text)
    {
        CurrentValue = value;
        SelectedText = text;
        await CloseDropdown();
        await InvokeAsync(StateHasChanged);
    }

    public async Task OpenDropdown()
    {
        if (Disabled) return;

        State = "open";

        if (_jsSelectModule is not null)
        {
            _jsSelectCleaner = await _jsSelectModule.InvokeAsync<IJSObjectReference>(
                "InitializeSelect",
                SelectTriggerRef,
                SelectContentRef,
                _placements[Placement],
                _csSelectRef,
                nameof(CloseDropdown),
                nameof(HandleKeyboardSelect)
            );
        }
    }

    [JSInvokable]
    public async Task CloseDropdown()
    {
        State = "closed";

        if (_jsSelectCleaner is not null)
        {
            await _jsSelectCleaner.InvokeVoidAsync("CleanUp");
            _jsSelectCleaner = null;
        }

        await InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public async Task HandleKeyboardSelect(int index)
    {
        if (index >= 0 && index < _items.Count)
        {
            var item = _items[index];
            if (!item.Disabled)
            {
                await SelectItemAsync(item.Value, item.GetDisplayText());
            }
        }
    }

    public async Task ToggleState()
    {
        if (State == "open")
        {
            await CloseDropdown();
        }
        else
        {
            await OpenDropdown();
        }
    }

    public void Dispose()
    {
        _csSelectRef?.Dispose();
        _jsSelectModule?.DisposeAsync();
        _jsSelectCleaner?.DisposeAsync();
    }
}
