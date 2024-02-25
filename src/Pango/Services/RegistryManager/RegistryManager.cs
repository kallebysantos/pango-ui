using Pango.Abstractions;
using Pango.Extensions;
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
            .Filter(dir => dir.Name == Path.GetFileNameWithoutExtension(metadataInput.LocalComponentPath))
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
        => Result.TryFrom(() => new FileInfo(path))
            .Ok()
            .Filter(file => file.Exists)
            .Filter(file => file.Extension == ".razor")
            .Map(file => new ComponentMetadata(
                Name: ResolveComponentName(file),
                Source: ResolveComponentSource(file),
                Files: [file.Name]
            ))
            .OkOr<IRegistryError>(new InvalidComponentPathError());

    protected static string ResolveComponentName(FileInfo componentFile)
        => componentFile.Name
            .Replace(componentFile.Extension, string.Empty)
            .ToKebabCase();

    protected string ResolveComponentSource(FileInfo componentFile)
        => Path.Combine(
            options.RegistryBaseUri.ToString(),
            componentFile.Name.Replace(componentFile.Extension, string.Empty)
        );
}