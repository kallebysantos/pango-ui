using System.Net;
using Pango.Abstractions;
using Pango.Abstractions.ErrorKinds;
using Pango.Types;
using Pango.Extensions;

namespace Pango.Services.RegistryClient;

public interface IComponentError;

public interface IComponentState;

public record struct UnResolved : IComponentState;

public record struct Resolved : IComponentState;

public record struct Streaming(Stream FileStream, string FileName) : IComponentState;

public record struct Downloaded : IComponentState;

public record struct Component<TState>(
    ComponentMetadata Metadata,
    TState State
)
    where TState : IComponentState, new()
{
    public Component(ComponentMetadata Metadata) : this(Metadata, new()) { }
}

public static class Component
{

    /// <summary>
    /// Resolves a component metadata by fetching the registry
    /// </summary>
    /// <param name="httpClient">A HttpClient instance</param>
    /// <param name="remoteUri">The registry uri to fetch</param>
    /// <param name="name">The name of the component to be resolved</param>
    /// <returns>The resolved component</returns>
    public static async Task<Result<Component<Resolved>, IError>> ResolveFrom(
        HttpClient httpClient,
        string remoteUri,
        string name
    )
    {
        var componentMetadataName = Path.ChangeExtension(name, "json");
        var componentMetadataUrl = Path.Combine(remoteUri, componentMetadataName);

        var response = await httpClient.GetAsync(componentMetadataUrl);

        var result = await response.ToResult()
            .MapErr(err => err.HttpResponse.StatusCode == HttpStatusCode.NotFound
                ? new NotFoundError().WithMessage("Component could not be found or not exists!")
                : err.Error
            )
            .AndThen(HttpClientExtensions.ReadFromJsonAsync<ComponentMetadata>);

        return result.Map(metadata => new Component<Resolved>(metadata));
    }
}

public static class ResolvedComponent
{
    /// <summary>
    /// Enumerates the component source and yield the files as streams
    /// </summary>
    /// <param name="component"></param>
    /// <param name="httpClient"></param>
    /// <returns></returns>
    public static async IAsyncEnumerable<Result<Component<Streaming>, IError>> GetComponentStreams(
        this Component<Resolved> component,
        HttpClient httpClient
    )
    {
        var GetFileUrl = (string file) => Path.Combine(component.Metadata.Source, file);
        var GetFileStream = (string fileUrl) => Result.TryFrom(() => httpClient.GetStreamAsync(fileUrl));

        foreach (var filename in component.Metadata.Files)
        {
            var fileUrl = GetFileUrl(filename);
            var streamResult = await GetFileStream(fileUrl);

            yield return streamResult
                .MapErr(ExceptionError.From)
                .Map(stream => new Component<Streaming>(
                    Metadata: component.Metadata,
                    State: new(
                        FileStream: stream,
                        FileName: filename
                    )
                ));
        }
    }
}