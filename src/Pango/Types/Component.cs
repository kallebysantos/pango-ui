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

public record struct Streaming(Stream FileStream) : IComponentState;

public record struct Downloaded : IComponentState;

public record struct Component<TState>(
    ComponentMetadata Metadata,
    TState State
)
    where TState : IComponentState, new()
{
    public Component(ComponentMetadata Metadata) : this(Metadata, new()) { }
}
