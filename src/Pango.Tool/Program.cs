using Pango.Commands;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config => 
{
    config.AddCommand<ComponentRegisterCreation>("registry-create-metadata")
        .WithDescription("Register a component from your folder to somewhere else.")
        .WithExample("registry-create", "raw.github.com/")
        .WithExample("registry-create", "raw.github.com/", "--component", "Button");
});

return app.Run(args);

// Uri registryUri = new("https://raw.githubusercontent.com/kallebysantos/pango-ui/HEAD/src/Pango.Components/UI");

// var registry = new RegistryManager(
//     options: new(registryUri)
// );

// var component = registry.CreateComponentMetadata(new(
//     LocalComponentPath: args.FirstOrDefault("./src/Pango.Components/UI/Button"))
// );

// Console.WriteLine($"Result: {component}\n\n");
// Console.WriteLine($"Json: {JsonSerializer.Serialize(component.Expect())}");

// var rootCommand = new RootCommand(
//     description: "Capture ants... I mean, components to your project"
// );

// var parser = new CommandLineBuilder(rootCommand)
//     .UseDefaults()
//     .UseHelp(ctx =>
//     {
//         #region Logo design
//         ctx.HelpBuilder.CustomizeLayout(
//                 _ =>
//                     HelpBuilder.Default
//                         .GetLayout()
//                         .Skip(1) // Skip the default command description section.
//                         .Prepend(_ => AnsiConsole.Write(@"
//                                              _/\/\/\/\/\/\-_
//  ____                                     _-//\/\/\/\/\/\/\/\>
// |  _ \    __ _   _ __     __ _    ___    /  \/\/\/\/\/\/\/\/\\>
// | |_) |  / _` | | '_ \   / _` |  / _ \  / .  \/\/\/\/\/\/\/\/\\>
// |  __/  | (_| | | | | | | (_| | | (_) | |    __--\__--/\/\/\/\/>
// |_|      \__,_| |_| |_|  \__, |  \___/  \__//  \| |\_____/   //
//                          |___/                 (_(_\________//"))
//         );
//         #endregion
//     })
//     .Build();

// await parser.Parse(args).InvokeAsync();