using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using Pango.Types;
using Pango.Abstractions;

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

        savedConfig
            .Inspect(result => AnsiConsole.MarkupLineInterpolated($"[bold grey]Saved:[/] {result}."))
            .InspectErr(err => AnsiConsole.MarkupLineInterpolated($"[bold red]Fail: {err}[/]."));

        return Convert.ToInt32(savedConfig.IsOk());
    }

    public override ValidationResult Validate(CommandContext context, ConfigurationInitSettings settings)
    {
        settings.Namespace ??= AskNamespace();
        settings.Output ??= AskOutput(defaultValue: "./" + string.Join('/', settings.Namespace.Split('.').Skip(1)));

        settings.RegistryUri ??= AskRegistryUri(defaultValue: "https://kallebysantos.github.io/pango-ui");

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
}