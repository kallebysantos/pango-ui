using System.Text.Json;
using Pango.Abstractions;

namespace Pango.Types;

public record struct LocalConfig
{
    public required string RegistrySchemaUri { get; set; }

    public required string TargetComponentNamespace { get; set; }

    public required string LocalComponentPath { get; set; }
}

public static class LocalConfigImpl
{
    public static async Task<Result<string, IError>> PersistLocalConfigFile(this LocalConfig config, string filepath)
    => (await Result.TryFrom(async () =>
        {
            await using var fileStream = File.Create(filepath);
            await JsonSerializer.SerializeAsync(fileStream, value: config);

            return fileStream.Name;
        })
    ).MapErr(ExceptionError.From);

}