using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class Calendar<TValue> : InputBase<TValue>
{
    [DisallowNull] public ElementReference? Element { get; protected set; }
    [Parameter] public InputDateType Type { get; set; } = InputDateType.Date;
    [Parameter] public string ParsingErrorMessage { get; set; } = string.Empty;

    [Inject] protected TwMerge TwMerge { get; set; } = null!;

    /// <summary>
    /// Merge Tailwind CSS classes without style conflicts
    /// </summary>
    private string? Tw(params string?[] classNames) => TwMerge.Merge(classNames);

    private string _typeAttributeValue = null!;
    private string _format = null!;
    private string _parsingErrorMessage = null!;

    private const string DateFormat = "yyyy-MM-dd"; // Compatible with HTML 'date' inputs
    private const string DateTimeLocalFormat = "yyyy-MM-ddTHH:mm:ss"; // Compatible with HTML 'datetime-local' inputs
    private const string MonthFormat = "yyyy-MM"; // Compatible with HTML 'month' inputs
    private const string TimeFormat = "HH:mm:ss"; // Compatible with HTML 'time' inputs

    private static readonly Dictionary<DayOfWeek, string> ColWeekShift = new()
    {
        { DayOfWeek.Sunday, "" },
        { DayOfWeek.Monday, "col-start-2" },
        { DayOfWeek.Tuesday, "col-start-3" },
        { DayOfWeek.Wednesday, "col-start-4" },
        { DayOfWeek.Thursday, "col-start-5" },
        { DayOfWeek.Friday, "col-start-6" },
        { DayOfWeek.Saturday, "col-start-7" },
    };

    public Calendar()
    {
        Type type = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

        if (type != typeof(DateTime) &&
            type != typeof(DateTimeOffset) &&
            type != typeof(DateOnly) &&
            type != typeof(TimeOnly) &&
            type != typeof(CalendarDateRange))
        {
            throw new InvalidOperationException($"Unsupported {GetType()} type param '{type}'.");
        }
    }

    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out TValue result,
        [NotNullWhen(false)] out string? validationErrorMessage)
    {
        if (BindConverter.TryConvertTo(value, CultureInfo.InvariantCulture, out result))
        {
            Debug.Assert(result != null);
            validationErrorMessage = null;
            return true;
        }

        validationErrorMessage = string.Format(CultureInfo.InvariantCulture, _parsingErrorMessage,
            DisplayName ?? FieldIdentifier.FieldName);
        return false;
    }

    private static IEnumerable<DateTime> GetEachDayOfInterval(DateTime start, DateTime end)
    {
        for (DateTime day = start; day <= end; day = day.AddDays(1))
        {
            yield return day;
        }
    }

    private bool IsSameMonth(DateTime day)
        => day.Month == _monthStart.Month;

    private static TValue? CastToTValue(DateTime day)
    {
        Type type = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

        if (type == typeof(DateTime))
        {
            return (TValue)(object)day;
        }

        if (type == typeof(DateOnly))
        {
            return (TValue)(object)DateOnly.FromDateTime(day);
        }

        throw new InvalidOperationException($"Unsupported type param '{type}'.");
    }

    private static DateTime? CastToDateTime(TValue? value)
    {
        Type type = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

        if (type == typeof(DateTime))
        {
            return (DateTime?)(object?)value;
        }

        if (type == typeof(DateOnly))
        {
            return ((DateOnly?)(object?)value)?.ToDateTime(TimeOnly.MinValue);
        }

        return null;
    }

    private DateTime GetExtendedDateAtLastWeek(DateTime date, DayOfWeek lastWeekDay)
    {
        int daysToAdd = (7 + lastWeekDay - date.DayOfWeek) % 7;
        DateTime extendedDate = date.AddDays(daysToAdd);

        if (FixedHeight && extendedDate.Month == date.Month)
        {
            extendedDate = extendedDate.AddDays(7);
        }

        return extendedDate;
    }

    private static DateTime GetExtendedDateAtFirstWeek(DateTime day, DayOfWeek firstWeekDay)
    {
        int daysToAdd = (day.DayOfWeek - firstWeekDay + 7) % 7;
        return day.AddDays(-daysToAdd);
    }
}

public class CalendarDateRange
{
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
