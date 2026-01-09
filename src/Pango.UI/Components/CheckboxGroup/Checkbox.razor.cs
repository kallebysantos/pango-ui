using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Pango.UI.Components;

public partial class Checkbox : InputCheckbox
{
    [CascadingParameter] private CheckboxGroupContent? Content { get; set; }
    private bool _previousValue;
    private bool _isHandlingChange;

    protected override void OnInitialized()
    {
        Content?.Group.RegisterCheckbox(this);
    }
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (Content is null || _isHandlingChange) return;

        if (CurrentValue != _previousValue)
        {
            _isHandlingChange = true;
            _previousValue = CurrentValue;
            await Content.Group.OnChildValueChanged();
            _isHandlingChange = false;
        }
    }

    internal async Task SetValueAsync(bool value)
    {
        CurrentValue = value;
        await InvokeAsync(StateHasChanged);
    }
}
