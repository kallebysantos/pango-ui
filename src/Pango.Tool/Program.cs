using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using Spectre.Console;

var rootCommand = new RootCommand(
    description: "Capture ants... I mean, components to your project"
);

var parser = new CommandLineBuilder(rootCommand)
    .UseDefaults()
    .UseHelp(ctx =>
    {
        #region Logo design
        ctx.HelpBuilder.CustomizeLayout(
                _ =>
                    HelpBuilder.Default
                        .GetLayout()
                        .Skip(1) // Skip the default command description section.
                        .Prepend(_ => AnsiConsole.Write(@"
                                             _/\/\/\/\/\/\-_
 ____                                     _-//\/\/\/\/\/\/\/\>
|  _ \    __ _   _ __     __ _    ___    /  \/\/\/\/\/\/\/\/\\>
| |_) |  / _` | | '_ \   / _` |  / _ \  / .  \/\/\/\/\/\/\/\/\\>
|  __/  | (_| | | | | | | (_| | | (_) | |    __--\__--/\/\/\/\/>
|_|      \__,_| |_| |_|  \__, |  \___/  \__//  \| |\_____/   //
                         |___/                 (_(_\________//"))
        );
        #endregion
    })
    .Build();

await parser.Parse(args).InvokeAsync();