using Microsoft.AspNetCore.Components;

namespace Pango.UI.Components;

public partial class CheckboxGroup : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private readonly List<Checkbox> _children = [];
    internal bool IsIndeterminate => _children.Any(c => c.Value) && _children.Any(c => !c.Value);

    internal void RegisterCheckbox(Checkbox checkbox)
    {
        if (!_children.Contains(checkbox))
            _children.Add(checkbox);
    }

    internal async Task OnValueChanged(bool value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(value);

        foreach (Checkbox child in _children)
            await child.SetValueAsync(value);

        await InvokeAsync(StateHasChanged);
    }

    internal async Task OnChildValueChanged()
    {
        if (_children.All(c => c.Value))
        {
            Value = true;
            await ValueChanged.InvokeAsync(true);
        }
        else if (_children.All(c => !c.Value))
        {
            Value = false;
            await ValueChanged.InvokeAsync(false);
        }

        await InvokeAsync(StateHasChanged);
    }
}
