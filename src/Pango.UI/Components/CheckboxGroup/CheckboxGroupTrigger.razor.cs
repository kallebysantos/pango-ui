using Microsoft.AspNetCore.Components;

namespace Pango.UI.Components;

public partial class CheckboxGroupTrigger : ComponentBase
{

    [CascadingParameter]
    internal CheckboxGroup Group { get; set; }

    [Parameter]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}
