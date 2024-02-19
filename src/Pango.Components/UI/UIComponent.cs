using Microsoft.AspNetCore.Components;

namespace Pango.Components.UI;

public class UIComponent : ComponentBase
{
    /// <summary>
    /// Gets or sets a collection of additional attributes that will be applied to the created element.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }
}