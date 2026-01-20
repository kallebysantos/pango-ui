using Microsoft.AspNetCore.Components;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class SelectTrigger
{
    [Inject] protected TwMerge TwMerge { get; set; } = null!;

    /// <summary>
    /// Merge Tailwind CSS classes without style conflicts
    /// </summary>
    private string? Tw(params string?[] classNames) => TwMerge.Merge(classNames);

    public enum Sizes
    {
        Default,
        Sm,
    }
}
