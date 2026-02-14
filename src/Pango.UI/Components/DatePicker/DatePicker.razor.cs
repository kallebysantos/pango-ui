using Microsoft.AspNetCore.Components;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class DatePicker<TValue>
{
    [Parameter] public bool WithMonthAndYearSelection { get; set; }
    [Parameter] public CalendarDateRangeBehavior Behavior { get; set; } = CalendarDateRangeBehavior.KeepFromMoveTo;
    [Parameter] public bool Required { get; set; }
    [Parameter] public DateTime MinDate { get; set; } = DateTime.MinValue;
    [Parameter] public DateTime MaxDate { get; set; } = DateTime.MaxValue;
    [Parameter] public bool FixedHeight { get; set; } = true;
    [Parameter] public DayOfWeek WeekStart { get; set; } = DayOfWeek.Sunday;

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
    private string? Tw(params string?[] classNames) => TwMerge.Merge(classNames);
}

public enum Sizes
{
    Default,
    Sm,
}
