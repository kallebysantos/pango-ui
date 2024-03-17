using System.Net.Http.Json;
using System.Text.Json;
using Pango.Abstractions;

namespace Pango.Extensions;

public record struct HttpError(HttpResponseMessage HttpResponse, IError Error) : IError
{
    public readonly IError GetError() => Error;
}

internal static class HttpClientExtensions
{
    public static Result<HttpResponseMessage, HttpError> ToResult(this HttpResponseMessage response)
        => Result.TryFrom(response.EnsureSuccessStatusCode)
            .MapErr(ExceptionError.From)
            .MapErr(error => new HttpError(response, error));

    public static Task<Result<T, IError>> ReadFromJsonAsync<T>(this HttpResponseMessage response)
        => response.Content.ReadFromJsonAsync<T>();

    public static Task<Result<T, IError>> ReadFromJsonAsync<T>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default
    )
        => response.Content.ReadFromJsonAsync<T>(cancellationToken);

    public static Task<Result<T, IError>> ReadFromJsonAsync<T>(this HttpContent content, CancellationToken cancellationToken = default)
        => ReadFromJsonAsync<T>(content, options: null, cancellationToken);

    public static async Task<Result<T, IError>> ReadFromJsonAsync<T>(
        this HttpContent content,
        JsonSerializerOptions? options,
        CancellationToken cancellationToken = default
    )
        => (await Result.TryFrom(() =>
                HttpContentJsonExtensions.ReadFromJsonAsync<T>(content, options, cancellationToken) as Task<T>))
            .MapErr(ExceptionError.From);
}