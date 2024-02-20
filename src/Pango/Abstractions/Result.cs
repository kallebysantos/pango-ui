namespace Pango.Abstractions;

public record Result<T, E>
{
    internal Option<T> OkValue { get; init; } = Option.None<T>();

    internal Option<E> ErrValue { get; init; } = Option.None<E>();

    internal Result() { }

    protected Result(T Value) : this()
    {
        OkValue = Option.Some(Value);
        ErrValue = Option.None<E>();
    }

    protected Result(E Error) : this()
    {
        ErrValue = Option.Some(Error);
        OkValue = Option.None<T>();
    }

    internal Result(Option<T> OkValue, Option<E> ErrValue) : this()
    {
        this.OkValue = OkValue;
        this.ErrValue = ErrValue;
    }

    public static implicit operator Result<T, E>(T value) => new Ok<T, E>(value);

    public static implicit operator Result<T, E>(E Error) => new Err<T, E>(Error);

    public override string ToString()
    {
        if (IsOk())
        {
            return $"Ok: {OkValue}";
        }
        return $"Err: {ErrValue}";
    }

    public Option<T> Ok() => OkValue;

    public Option<E> Err() => ErrValue;

    public bool IsOk() => OkValue.IsSome() && ErrValue.IsNone();

    public bool IsErr() => ErrValue.IsSome();

    public Result<U, E> Map<U>(Func<T, U> ok)
        => new(OkValue.Map(ok), ErrValue);

    public Result<T, F> MapErr<F>(Func<E, F> err)
        => new(OkValue, ErrValue.Map(err));


    public void Match(Action<T> ok, Action<E> err)
    {
        if (this is { OkValue: Some<T> okValue })
            ok(okValue);

        if (this is { ErrValue: Some<E> errValue })
            err(errValue);
    }

    public U Match<U>(Func<T, U> ok, Func<Option<E>, U> err)
    {
        if (this is { OkValue: Some<T> okValue })
            return ok(okValue);

        return err(ErrValue);
    }

    public Result<U, E> AndThen<U>(Func<T, Result<U, E>> op)
        => this is { OkValue: Some<T> okValue }
        ? op(okValue)
        : new(new None<U>(), ErrValue);

    public async Task<Result<U, E>> AndThen<U>(Func<T, Task<Result<U, E>>> op)
        => this is { OkValue: Some<T> okValue }
        ? await op(okValue)
        : await Task.FromResult<Result<U, E>>(new(new None<U>(), ErrValue));

    public T UnwrapOr(T @default)
        => OkValue.UnwrapOr(@default);
}

public static class Result
{
    public static Ok<T, E> Ok<T, E>(T Value) => new(Value);

    public static Err<T, E> Err<T, E>(E Value) => new(Value);

    public static Result<T, Exception> TryFrom<T>(Func<T> @delegate)
    {
        try
        {
            return @delegate();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static T Expect<T, E>(this Result<T, E> result)
        => Expect(result, $"Expect: {nameof(result)} to be {nameof(Ok)} but found {nameof(Err)}");

    public static T Expect<T, E>(this Result<T, E> result, string message)
        => Expect(result, new Exception(message));

    public static T Expect<T, E>(this Result<T, E> result, Exception ex)
        => result.OkValue.Expect(ex);

    public static E ExpectErr<T, E>(this Result<T, E> result)
        => ExpectErr(result, $"Expect: {nameof(result)} to be {nameof(Err)} but found {nameof(Ok)}");

    public static E ExpectErr<T, E>(this Result<T, E> result, string message)
        => ExpectErr(result, new Exception(message));

    public static E ExpectErr<T, E>(this Result<T, E> result, Exception ex)
        => result.ErrValue.Expect(ex);

}

public sealed record Ok<T, E>(T Value) : Result<T, E>(Value)
{
    public static implicit operator T(Ok<T, E> ok) => ok.Value;

    public static implicit operator Ok<T, E>(T value) => new(value);
}

public record Err<T, E>(E Value) : Result<T, E>(Value)
{
    public static implicit operator E(Err<T, E> err) => err.Value;

    public static implicit operator Err<T, E>(E err) => new(err);
}

public record struct OkResult();
