using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TailwindMerge;

namespace Pango.UI.Components;

public partial class Calendar<TValue> : InputBase<TValue>
{
    [DisallowNull] public ElementReference? Element { get; protected set; }
    [Parameter] public string ParsingErrorMessage { get; set; } = string.Empty;

    [Inject] protected TwMerge TwMerge { get; set; } = null!;

    /// <summary>
    /// Merge Tailwind CSS classes without style conflicts
    /// </summary>
    private string? Tw(params string?[] classNames) => TwMerge.Merge(classNames);

    private readonly bool _isRangeMode;

    public Calendar()
    {
        Type type = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

        if (type != typeof(DateTime) &&
            type != typeof(DateOnly) &&
            type != typeof(CalendarDateRange))
        {
            throw new InvalidOperationException($"Unsupported {GetType()} type param '{type}'.");
        }

        _isRangeMode = type == typeof(CalendarDateRange);
    }

    protected override bool TryParseValueFromString(
        string? value,
        [MaybeNullWhen(false)] out TValue result,
        [NotNullWhen(false)] out string? validationErrorMessage)
    {
        if (BindConverter.TryConvertTo(value, CultureInfo.InvariantCulture, out result))
        {
            Debug.Assert(result != null);
            validationErrorMessage = null;
            return true;
        }

        validationErrorMessage = string.Format(CultureInfo.InvariantCulture, ParsingErrorMessage,
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

    private int? GetColStart(DayOfWeek dayOfWeek)
    {
        int colStart = ((int)dayOfWeek - (int)WeekStart + 7) % 7 + 1;
        return colStart == 1 ? null : colStart;
    }

    private string[] GetWeekDaysNames()
    {
        string[] dayNames = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
        int startIndex = (int)WeekStart;

        return dayNames.Skip(startIndex)
            .Concat(dayNames.Take(startIndex))
            .ToArray();
    }

    private bool IsSameMonth(DateTime day)
        => day.Month == _monthStart.Month;

    private static TValue? CastToTValue(DateTime from, DateTime? to = null)
    {
        Type type = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

        if (type == typeof(DateTime))
        {
            return (TValue)(object)from;
        }

        if (type == typeof(DateOnly))
        {
            return (TValue)(object)DateOnly.FromDateTime(from);
        }

        if (type == typeof(CalendarDateRange))
        {
            CalendarDateRange newDateRange = new() { From = from, To = to };
            return (TValue)(object)newDateRange;
        }

        throw new InvalidOperationException($"Unsupported type param '{type}'.");
    }

    private static (DateTime?, DateTime?) CastToDateTime(TValue? value)
    {
        Type type = Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue);

        if (type == typeof(DateTime))
        {
            return ((DateTime?)(object?)value, null);
        }

        if (type == typeof(DateOnly))
        {
            return (((DateOnly?)(object?)value)?.ToDateTime(TimeOnly.MinValue), null);
        }

        if (type == typeof(CalendarDateRange))
        {
            CalendarDateRange? range = (CalendarDateRange?)(object?)value;
            return (range?.From, range?.To);
        }

        return (null, null);
    }

    private DateTime GetExtendedDateAtLastWeek(DateTime date, DayOfWeek lastWeekDay)
    {
        int daysToAdd = (7 + lastWeekDay - date.DayOfWeek) % 7;
        DateTime extendedDate = date.AddDays(daysToAdd);

        if (FixedHeight && extendedDate.Month == date.Month)
        {
            extendedDate = extendedDate.AddDays(7);
        }

        if (extendedDate > DateTime.MaxValue)
        {
            extendedDate = DateTime.MaxValue;
        }

        return extendedDate;
    }

    private static DateTime GetExtendedDateAtFirstWeek(DateTime day, DayOfWeek firstWeekDay)
    {
        int daysToAdd = (day.DayOfWeek - firstWeekDay + 7) % 7;
        DateTime result = day.AddDays(-daysToAdd);

        if (result < DateTime.MinValue)
        {
            result = DateTime.MinValue;
        }

        return result;
    }

    private bool CheckPreviousMonth()
    {
        DateTime date;

        if (DateTime.DaysInMonth(_monthStart.Year, _monthStart.Month) > MinDate.Day)
        {
            date = new(_monthStart.Year, _monthStart.Month, MinDate.Day);
        }
        else
        {
            date = new(_monthStart.Year, _monthStart.Month, DateTime.DaysInMonth(_monthStart.Year, _monthStart.Month));
        }

        date = date.AddMonths(-1);
        return date >= MinDate && _daysOfCurrentMonth.All(d => d != date);
    }

    private bool CheckNextMonth()
    {
        DateTime date;

        if (DateTime.DaysInMonth(_monthEnd.Year, _monthEnd.Month) > MaxDate.Day)
        {
            date = new(_monthEnd.Year, _monthEnd.Month, MaxDate.Day);
        }
        else
        {
            date = new(_monthEnd.Year, _monthEnd.Month, DateTime.DaysInMonth(_monthEnd.Year, _monthEnd.Month));
        }

        date = date.AddMonths(1);
        return date <= MaxDate && _daysOfCurrentMonth.All(d => d != date);
    }

    private bool IsDaySelected(DateTime day)
    {
        (DateTime? from, DateTime? to) = CastToDateTime(CurrentValue);
        return day == from || day == to;
    }

    private bool IsBetweenSelection(DateTime day)
    {
        (DateTime? from, DateTime? to) = CastToDateTime(CurrentValue);
        return day > from && day < to;
    }

    private bool IsFromDate(DateTime day)
    {
        (DateTime? from, DateTime? to) = CastToDateTime(CurrentValue);
        return IsDaySelected(day) && day == from && to is not null && _isRangeMode;
    }

    private bool IsToDate(DateTime day)
    {
        (_, DateTime? to) = CastToDateTime(CurrentValue);
        return IsDaySelected(day) && day == to && _isRangeMode;
    }
}

public enum CalendarDateRangeBehavior
{
    // Google Flights behavior, each click selects a new range
    AlwaysSelectNewRange,

    //Shadcn ui / react-day-picker behavior, clicking inside the range only changes the To value
    KeepFromMoveTo,
}

public sealed class CalendarDateRange
{
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
