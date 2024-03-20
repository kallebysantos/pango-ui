using Spectre.Console;
using Spectre.Console.Cli;

using Pango.Commands;
using System.Reflection;

var app = new CommandApp();
var version = Assembly.GetExecutingAssembly().GetName()?.Version ?? new();

app.Configure(config =>
{
    config.SetApplicationName("pango");
    config.SetApplicationVersion(version.ToString(3));

    // Display Pango logo
    if (args.Length == 0)
    {
        AnsiConsole.MarkupLine(@"
[gold1]                                         [/][orangered1]      _/\/\/\/\/\/\-_     [/]
[gold1]  ____                                   [/][orangered1]   _-//\/\/\/\/\/\/\/\>   [/]
[gold1] |  _ \    __ _   _ __     __ _    ___   [/][orangered1]  /  \/\/\/\/\/\/\/\/\\>  [/]
[gold1] | |_) |  / _` | | '_ \   / _` |  / _ \  [/][orangered1] / .  \/\/\/\/\/\/\/\/\\> [/]
[gold1] |  __/  | (_| | | | | | | (_| | | (_) | [/][orangered1] |    __--\__--/\/\/\/\/> [/]
[gold1] |_|      \__,_| |_| |_|  \__, |  \___/  [/][orangered1] \__//  \| |\_____/   //  [/]
[gold1]                          |___/          [/][orangered1]   /    (_(_\________//   [/]");
        AnsiConsole.MarkupInterpolated($"\n[grey]Pango v{version.ToString(3)}[/] - ");
        AnsiConsole.MarkupInterpolated($"[bold]Capture ants... I mean, components to your project[/]\n\n");
    }

    config.AddCommand<DownloadComponent>("add")
        .WithDescription("Downloads a component from the specified registry.")
        .WithExample("add", "button");

    config.AddCommand<ConfigurationInit>("init")
        .WithDescription("Creates a Pango configuration file.")
        .WithExample("init", "-n MyProject.Components", "-o src/MyProject.Components", "--registry-uri https://raw.github.com/repo");

    config.AddBranch("registry", cmd =>
    {
        cmd.SetDescription("Tools for managing registry");
        cmd.AddCommand<ComponentRegisterCreation>("create-metadata")
            .WithDescription("Creates registry metadata for the given components.")
            .WithExample("registry", "create-metadata", "Components/*", "-o", "dist/components", "--registry-uri", "https://raw.github.com/repo");
    });
});


return app.Run(args);
