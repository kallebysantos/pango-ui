namespace Pango.Abstractions.ErrorKinds;

public interface IOErrorKind : IError;

public readonly record struct FileTooLargeError() : IOErrorKind
{
    public Option<long> MaxSize { get; init; } = new None<long>();

    public Option<string> Message { get; init; } = new None<string>();

    public FileTooLargeError WithMaxSize(long value) => this with { MaxSize = value };

    public FileTooLargeError WithMessage(Func<FileTooLargeError, string> messageParser) => this with { Message = messageParser(this) };

    public override readonly string ToString() => Message.UnwrapOr(base.ToString() ?? string.Empty);
}

public readonly record struct NotFoundError() : IOErrorKind
{
    public Option<string> Message { get; init; } = new None<string>();

    public NotFoundError WithMessage(string message) => this with { Message = message };

    public override readonly string ToString() => Message.UnwrapOr(base.ToString() ?? string.Empty);
}

public readonly record struct PermissionDeniedError() : IOErrorKind
{
    public Option<string> Message { get; init; } = new None<string>();

    public PermissionDeniedError WithMessage(string message) => this with { Message = message };

    public override readonly string ToString() => Message.UnwrapOr(base.ToString() ?? string.Empty);
}