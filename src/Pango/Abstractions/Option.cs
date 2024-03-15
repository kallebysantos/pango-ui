namespace Pango.Abstractions;

public abstract record Option<T>()
{
    internal T? Value { get; init; }

    protected Option(T Value) : this()
    {
        this.Value = Value;
    }

    public static implicit operator Option<T>(T? value) =>
        (value is not null) ? new Some<T>(value) : new None<T>();

    public Result<T, E> OkOr<E>(E err) =>
        this is Some<T> some
        ? some.Value
        : err;
}

public record Some<T>(T Value) : Option<T>(Value)
{
    public new T Value { get; } = Value;

    public static implicit operator T(Some<T> some) => some.Value!;

    public static implicit operator Some<T>(T value) => new(value);
}

public sealed record None<T>() : Option<T>;

public static class Option
{
    public static Option<T> From<T>(T? Value) => Value is null ? None<T>() : Some(Value);

    public static Some<T> Some<T>(T Value) => new(Value);

    public static None<T> None<T>() => new();

    public static Option<T> Default<T>() => default(T);

    public static bool IsSome<T>(this Option<T> option) => option is Some<T>;

    public static bool IsNone<T>(this Option<T> option) => option is None<T>;


    public static Option<T> WhenSome<T, U>(this Option<T> option, Option<U> optionB)
        => option is Some<T> && optionB is Some<U>
            ? option
            : None<T>();

    public static Option<U> And<T, U>(this Option<T> option, Option<U> optionB)
        => option is Some<T> && optionB is Some<U>
            ? optionB
            : None<U>();

    public static Option<U> Map<U, T>(this Option<T> option, Func<T, U> some) =>
        option is Some<T> value
        ? some(value)
        : None<U>();

    /// <summary>
    /// Converts from Option<Option<T>> to Option<T>.
    /// </summary>
    public static Option<T> Flatten<T>(this Option<Option<T>> optionToFlatten)
        => optionToFlatten is { Value: Option<T> flatten }
            ? flatten
            : None<T>();

    public static Option<U> FlatMap<U, T>(this Option<T> option, Func<T, Option<U>> some)
        => option.Map(some).Flatten();

    public static U? MapOrDefault<T, U>(this Option<T> option, Func<T, U> some) =>
        option is { Value: T value }
        ? some(value)
        : default;

    public static U MapOrDefault<U, T>(this Option<T> option, Func<T, U> some, U @default) =>
        option is { Value: T value }
        ? some(value)
        : @default;

    /// <summary>
    /// Returns None if the option is None, otherwise calls predicate with the wrapped value and returns:
    /// Some(t) if predicate returns true (where t is the wrapped value), and
    /// None if predicate returns false.
    /// This function works similar to Iterator::filter(). You can imagine the Option<T> being an iterator over one or 
    /// zero elements. filter() lets you decide which elements to keep.
    /// </summary>
    public static Option<T> Filter<T>(this Option<T> option, Func<T, bool> predicate) =>
        option is { Value: T value } && predicate(value)
        ? Some(value)
        : None<T>();


    /// <summary>
    /// Takes the value out of the option, but only if the predicate evaluates to true on a mutable reference to the value.
    /// In other words, replaces self with None if the predicate returns true.
    /// </summary>
    public static Option<T> DiscardIf<T>(this Option<T> option, Func<T, bool> predicate)
    {
        if (option is Some<T> value && predicate(value))
        {
            return None<T>();
        }

        return option;
    }

    public static void Then<T>(this Option<T> option, Action<T> some)
    {
        if (option is { Value: T value })
            some(value);
    }

    public static void Match<T>(this Option<T> option, Action<T> some, Action none)
    {
        if (option is Some<T> value)
            some(value);
        else
            none();
    }

    public static U Match<U, T>(this Option<T> option, Func<T, U> some, Func<U> none) =>
        option is { Value: T value }
        ? some(value)
        : none();

    public static T Expect<T>(this Option<T> option) =>
        Expect(option, $"Expect: {nameof(option)} to be {nameof(Some)} but found {nameof(None)}");

    public static T Expect<T>(this Option<T> option, string message) =>
        Expect(option, new Exception(message));

    public static T Expect<T>(this Option<T> option, Exception ex) =>
        option.Value ?? throw ex;

    public static T UnwrapOr<T>(this Option<T> option, T @default) => option.Value ?? @default;
}