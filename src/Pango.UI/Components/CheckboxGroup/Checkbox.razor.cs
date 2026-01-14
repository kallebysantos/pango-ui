using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Pango.UI.Components;

public partial class Checkbox : InputCheckbox
{
    [CascadingParameter] private CheckboxGroupContent? Content { get; set; }

    protected override void OnInitialized()
    {
        Content?.Group.RegisterCheckbox(this);
    }

    internal async Task SetValueAsync(bool value)
    {
        CurrentValue = value;
        await InvokeAsync(StateHasChanged);
    }
}
