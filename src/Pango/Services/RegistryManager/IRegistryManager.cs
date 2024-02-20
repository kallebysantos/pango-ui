using Pango.Abstractions;
using Pango.Types;

namespace Pango.Services.RegistryManager;

public interface IRegistryError;

public record struct InvalidComponentPathError : IRegistryError;

public record struct CreateComponentMetadataInput(
    string LocalComponentPath
);

public interface IRegistryManager
{
    Result<ComponentMetadata, IRegistryError> CreateComponentMetadata(CreateComponentMetadataInput metadataInput);
}