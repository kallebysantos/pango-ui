using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class Input : InputText
{
    [Inject]
    protected TwMerge TwMerge { get; set; } = null!;

    /// <summary>
    /// Merge Tailwind CSS classes without style conflicts
    /// </summary>
    public string? Tw(params string?[] classNames) => TwMerge.Merge(classNames);
}
