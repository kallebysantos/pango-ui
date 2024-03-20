using Spectre.Console.Cli;
using Pango.Abstractions;
using Pango.Services.RegistryClient;
using System.Diagnostics.CodeAnalysis;
using ComponentModel = System.ComponentModel;
using Spectre.Console;
using System.Diagnostics;

namespace Pango.Commands;

public sealed class DownloadComponentSettings : CommandSettings
{
    [NotNull]
    [ComponentModel.Description("URI of the registry to fetch the component.")]
    [CommandOption("--registry-uri")]
    public required string RegistryUri { get; set; }

    [NotNull]
    [ComponentModel.Description("Name of the component to download.")]
    [CommandArgument(0, "<ARGUMENT>")]
    public required string ComponentName { get; set; }

    [NotNull]
    [ComponentModel.Description("Output folder for the component download.")]
    [CommandOption("-o|--output")]
    public required string Output { get; set; }

    [NotNull]
    [ComponentModel.Description("Target namespace.")]
    [CommandOption("-n|--namespace")]
    public required string Namespace { get; set; }
}

public sealed class DownloadComponent : AsyncCommand<DownloadComponentSettings>
{
    public override async Task<int> ExecuteAsync(
        [NotNull] CommandContext context,
        [NotNull] DownloadComponentSettings settings
    )
    {
        AnsiConsole.MarkupLineInterpolated($"[bold grey]Adding Component:[/] [underline]{settings.ComponentName}[/]");

        var httpClient = new HttpClient();
        var foundComponentResult = (await AnsiConsole
            .Status()
            .StartAsync("Fetching component metadata...", async ctx =>
                await Component.ResolveFrom(
                    httpClient: httpClient,
                    remoteUri: settings.RegistryUri,
                    name: settings.ComponentName
                )
            ))
            .Inspect(result => AnsiConsole.MarkupLineInterpolated($"[bold grey]Fetch:[/] found {result.Metadata.Files.Length} files for this component."))
            .InspectErr(err => AnsiConsole.MarkupLineInterpolated($"[bold red]Fail: {err}[/]."));

        if (foundComponentResult.Ok() is not Some<Component<Resolved>> component)
            return 1;

        await AnsiConsole
            .Status()
            .StartAsync("Downloading component files...", async ctx =>
            {
                await foreach (var item in component.Value.GetComponentStreams(httpClient))
                {
                    var downloadResult = await item
                        .Inspect(result => ctx.Status($"Download: {result.State.FileName}"))
                        .InspectErr(err => AnsiConsole.MarkupLineInterpolated($"[bold red]Fail: {err}[/]."))
                        .AndThen(async item =>
                            await item.Download(
                                localBaseComponentPath: settings.Output,
                                localBaseNamespace: settings.Namespace
                            )
                        );

                    downloadResult
                        .Inspect(result => AnsiConsole.MarkupLineInterpolated($"[bold grey]Saved:[/] {result.State.Filepath}"))
                        .InspectErr(err => AnsiConsole.MarkupLineInterpolated($"[bold red]Fail: {err}[/]."));
                }
            });

        return 0;
    }
}