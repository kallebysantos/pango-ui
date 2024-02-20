using System.Text.Json;
using Pango.Abstractions;
using Pango.Types;

namespace Pango.Services.RegistryManager;

public class RegistryManager(RegistryOptions options) : IRegistryManager
{
    public Result<ComponentMetadata, IRegistryError> CreateComponentMetadata(CreateComponentMetadataInput metadataInput)
    {
        var isComponentPath = Path.HasExtension(metadataInput.LocalComponentPath);

        if (isComponentPath)
            return ResolveComponent(metadataInput.LocalComponentPath);

        var componentFolder = Result.TryFrom(() => new DirectoryInfo(metadataInput.LocalComponentPath));
        if (componentFolder.IsErr())
            return new InvalidComponentPathError();

        return componentFolder.Ok()
            .Filter(dir => dir.Exists)
            .Map(dir => dir.EnumerateFiles("*.razor*"))
            .Filter(files => files.Any())
            .OkOr<IRegistryError>(new InvalidComponentPathError())
            .AndThen(files =>
                ResolveComponent(files.First(f => f.Extension.StartsWith(".razor")).FullName)
                .Map(component => component with
                {
                    Files = files.Select(f => f.Name).ToArray()
                })
            );
    }

    protected Result<ComponentMetadata, IRegistryError> ResolveComponent(string path)
    {
        var fileName = Option.From(Path.GetFileName(path));
        var fileExtension = Option.From(Path.GetExtension(path));
        var cleanName = Option.From(Path.GetFileNameWithoutExtension(path));

        return fileExtension
        .Filter(ext => ext.StartsWith(".razor"))
        .WhenSome(fileName)
        .And(cleanName)
        .Map(cleanName => new ComponentMetadata(
            Name: cleanName.ToLowerInvariant(),
            Source: Path.Combine(options.RegistryBaseUri.AbsolutePath, cleanName),
            Files: [fileName.Expect()]
        ))
        .OkOr<IRegistryError>(new InvalidComponentPathError());
    }
}