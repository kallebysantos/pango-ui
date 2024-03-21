using System.Text;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using Pango.Types;
using Pango.Extensions;
using Pango.Abstractions;
using Pango.Services.RegistryClient;

using StringExtensions = Pango.Extensions.StringExtensions;
namespace Pango.Commands;

public sealed class ConfigurationInitSettings : CommandSettings
{
    [NotNull]
    [Description("URI of the registry to fetch the component.")]
    [CommandOption("--registry-uri")]
    public string? RegistryUri { get; set; }

    [NotNull]
    [Description("Target namespace.")]
    [CommandOption("-n|--namespace")]
    public string? Namespace { get; set; }

    [NotNull]
    [Description("Target output path.")]
    [CommandOption("-o|--output")]
    public string? Output { get; set; }
}

public sealed class ConfigurationInit : AsyncCommand<ConfigurationInitSettings>
{
    static readonly string RegistryBaseDownloadUrl = "https://raw.githubusercontent.com/kallebysantos/pango-ui/main/src/Pango.Components";
    static readonly string RegistryBaseSchema = "https://kallebysantos.github.io/pango-ui";

    public override async Task<int> ExecuteAsync(
        [NotNull] CommandContext context,
        [NotNull] ConfigurationInitSettings settings
    )
    {
        var config = new LocalConfig()
        {
            RegistrySchemaUri = settings.RegistryUri,
            TargetComponentNamespace = settings.Namespace,
            LocalComponentPath = settings.Output
        };

        var savedConfig = await AnsiConsole
            .Status()
            .StartAsync(
                status: "Saving pango configuration file",
                func: _ => config.PersistLocalConfigFile(filepath: "./pango-ui.config.json")
            );

        await (await savedConfig
            .Inspect(result => AnsiConsole.MarkupLineInterpolated($"[bold grey]Saved: {result}[/]"))
            .InspectErr(err => AnsiConsole.MarkupLineInterpolated($"[bold red]Fail: {err}[/]."))
            .AndThen(async _ =>
                AnsiConsole.Confirm("Would like to add Tailwind Helper?")
                    ? await GetRegistryRawFile(settings.Namespace, settings.Output, "Utils/TailwindHelper.cs")
                    : new OkResult()
                ))
            .AndThen(async _ =>
                AnsiConsole.Confirm("Would like to add UI Component base class?")
                    ? await GetRegistryRawFile(settings.Namespace, settings.Output, "UI/UIComponent.cs")
                    : new OkResult()
                );

        return Convert.ToInt32(savedConfig.IsOk());
    }


    public override ValidationResult Validate(CommandContext context, ConfigurationInitSettings settings)
    {
        settings.Namespace ??= AskNamespace();
        settings.Output ??= AskOutput(defaultValue: "./" + string.Join('/', settings.Namespace.Split('.').Skip(1)));

        settings.RegistryUri ??= AskRegistryUri(defaultValue: RegistryBaseSchema);

        return base.Validate(context, settings);
    }

    static string AskRegistryUri(string defaultValue) => AnsiConsole
        .Confirm("Would like to set a component registry?[grey] Default: Pango UI[/]", defaultValue: false)
        ? AnsiConsole.Ask<string>("Enter the component registry url:")
        : defaultValue;

    static string AskNamespace()
    {
        var askMessage = "Enter the target namespace:";

        return Option.From(Value: new DirectoryInfo("./")
            .EnumerateFiles(searchPattern: "*.csproj")
            .FirstOrDefault()
        )
        .Map(file => string.Join('.', Path.GetFileNameWithoutExtension(file.Name), "UI"))
        .Match(
            some: projectFileName => AnsiConsole.Ask(askMessage, projectFileName),
            none: () => AnsiConsole.Ask<string>(askMessage)
        );
    }

    static string AskOutput(string defaultValue)
        => AnsiConsole.Ask("Enter the target output folder:", defaultValue);

    static async Task<Result<IOk, IError>> GetRegistryRawFile(
        string @namespace,
        string componentsFolder,
        string registryFilePath
    )
    {
        var registryFileUrl = Path.Combine(RegistryBaseDownloadUrl, registryFilePath);

        var destinationNamespace = StringExtensions.JoinMerge(
            separator: '.',
            values: [
                ..@namespace.Split('.'),
                ..registryFilePath.Replace(Path.GetFileName(registryFilePath), string.Empty).Split('/')
            ]);

        var destinationFilepath = StringExtensions.JoinMerge(
            separator: '/',
            values: [
                ..componentsFolder.Split('/'),
                ..registryFilePath.Split('/')
            ]);

        var destinationFileInfo = new FileInfo(
            fileName: destinationFilepath
        );

        destinationFileInfo.Directory?.Create();

        var httpClient = new HttpClient();

        return await AnsiConsole
            .Status()
            .StartAsync($"Downloading {destinationFileInfo.Name}...", async ctx =>
            {
                var fileStream = await Result.TryFrom(() => httpClient.GetStreamAsync(registryFileUrl));

                var downloadResult = await fileStream
                    .MapErr(ExceptionError.From)
                    .Inspect(result => ctx.Status($"Download: {registryFileUrl}"))
                    .InspectErr(err => AnsiConsole.MarkupLineInterpolated($"[bold red]Fail: {err}[/]."))
                    .AndThen(async item =>
                        await StreamingComponent.WriteComponentStream(
                            stream: item,
                            filepath: destinationFileInfo.FullName,
                            @namespace: destinationNamespace
                        )
                    );

                return downloadResult
                    .Inspect(result => AnsiConsole.MarkupLineInterpolated($"[bold grey]Saved: {destinationFileInfo.FullName}[/]"))
                    .InspectErr(err => AnsiConsole.MarkupLineInterpolated($"[bold red]Fail: {err}[/]."));

            });
    }
}