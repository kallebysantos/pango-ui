namespace Pango.Abstractions;

public record Result<T, E>
{
    internal Option<T> OkValue { get; set; } = Option.None<T>();

    internal Option<E> ErrValue { get; set; } = Option.None<E>();

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

    public Option<T> Ok() => OkValue;

    public Option<E> Err() => ErrValue;

    public bool IsOk() => OkValue.IsSome() && ErrValue.IsNone();

    public bool IsErr() => ErrValue.IsSome();

    public Result<U, E> Map<U>(Func<T, U> ok)
        => new(OkValue.Map(ok), ErrValue);

    public Result<T, F> MapErr<F>(Func<E, F> err)
        => new(OkValue, ErrValue.Map(err));

    public Result<T, E> ReplaceIf(Func<bool> predicate, T value)
    {
        if (predicate())
        {
            var old = new Result<T, E>(OkValue, ErrValue);
            OkValue = value;
            ErrValue = new None<E>();
            return old;
        }

        return this;
    }

    public void Match(Action<T> ok, Action<E> err)
    {
        if (this is { OkValue: Some<T> okValue })
            ok(okValue);

        if (this is { ErrValue: Some<E> errValue })
            err(errValue);
    }

    public U Match<U>(Func<T, U> ok, Func<E, U> err)
    {
        if (this is { OkValue: Some<T> okValue })
            return ok(okValue);

        return err(ErrValue!.UnwrapOr(default)!);
    }

    public async Task<U> Match<U>(Func<T, Task<U>> ok, Func<E, U> err)
    {
        if (this is { OkValue: Some<T> okValue })
            return await ok(okValue);

        return err(ErrValue!.UnwrapOr(default)!);
    }

    public void Then(Action<T> ok, Action<E> err)
    {
        if (this is { OkValue: Some<T> okValue })
        {
            ok(okValue);
            return;
        }

        err(ErrValue!.UnwrapOr(default)!);
    }

    public void Then(Action<Result<T, E>> op)
        => op(this);

    public Result<U, E> AndThen<U>(Func<T, Result<U, E>> op)
        => this is { OkValue: Some<T> okValue }
        ? op(okValue)
        : new(new None<U>(), ErrValue);

    public async Task<Result<U, E>> AndThen<U>(Func<T, Task<Result<U, E>>> op)
        => this is { OkValue: Some<T> okValue }
        ? await op(okValue)
        : await Task.FromResult<Result<U, E>>(new(new None<U>(), ErrValue));

    public Result<T, E> Inspect(Action<T> op)
    {
        if (this is { OkValue: Some<T> okValue })
            op(okValue);

        return this;
    }

    public Result<T, E> InspectErr(Action<E> op)
    {
        if (this is { ErrValue: Some<E> errValue })
            op(errValue);

        return this;
    }

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

    public static Result<T, IError> TryFrom<T, TIError>(Func<T> @delegate) where TIError : IError
        => TryFrom(@delegate)
            .MapErr(ExceptionError.From);


    public static async Task<Result<T, Exception>> TryFrom<T>(Func<Task<T>> @delegate)
    {
        try
        {
            return await @delegate();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static Result<U, E> And<U, T, E>(this Result<T, E> result, Result<U, E> resultB)
    => result.IsOk()
    ? resultB
    : new(new None<U>(), result.ErrValue);

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

public interface IOk;

public interface IError;

public record struct OkResult : IOk;

public record struct ExceptionError(Exception Exception) : IError
{
    public static IError From(Exception exception) => new ExceptionError(exception);
}