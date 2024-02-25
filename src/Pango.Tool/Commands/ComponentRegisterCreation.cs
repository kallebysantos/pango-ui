using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;

using Spectre.Console;
using Spectre.Console.Cli;

using Pango.Abstractions;
using Pango.Services.RegistryManager;

namespace Pango.Commands;

public sealed class ComponentRegisterCreation : AsyncCommand<ComponentRegisterCreation.Settings>
{
    private static readonly TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

    public sealed class Settings : CommandSettings
    {
        [NotNull]
        [Description("URI to upload with your components.")]
        [CommandOption("--registry-uri")]
        public required string RegistryUri { get; set; }

        [NotNull]
        [Description("Folder with multiple components or single component.")]
        [CommandArgument(0, "<ARGUMENT>")]
        public required string[] ComponentsSource { get; set; }

        [NotNull]
        [Description("Output folder for the components registration.")]
        [CommandOption("-o|--output")]
        public required string Output { get; set; }
    }

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        var registryUri = new Uri(settings.RegistryUri);
        var registryManager = new RegistryManager(new(registryUri));

        var components = settings.ComponentsSource
            .Select(src => new CreateComponentMetadataInput(src))
            .Select(registryManager.CreateComponentMetadata)
            .Where(result => result.IsOk())
            .Select(result => result.Expect());

        var outputDir = new DirectoryInfo(settings.Output);
        if (!outputDir.Exists)
            outputDir.Create();

        foreach (var component in components)
        {
            var metadataFileName = Path.ChangeExtension(component.Name, ".json");
            var metadataFilePath = Path.Combine(outputDir.FullName, metadataFileName);
            await using var metadataFile = File.Create(metadataFilePath);
            await JsonSerializer.SerializeAsync(
                utf8Json: metadataFile,
                value: component,
                options: new(JsonSerializerDefaults.Web)
            );
        }

        return 0;

        //var componentsJson = await ServiceHttp.GetComponentsJsonAsync();
        // var path = CommandsHelper.AllOrSingleComponent(settings.ComponentsSource);


        /*         List<ComponentMetadata> components = []; */

        /* if(path is AllComponents searchComponent)
        {
            AnsiConsole.WriteLine("All: " + searchComponent.Path);

            var directories = Directory.EnumerateDirectories(searchComponent.Path, "*");
            var files = Directory.EnumerateFiles(searchComponent.Path, "*.razor*");

            foreach(var dir in directories)
            {
                var comp = registryManager.CreateComponentMetadata(new(dir));
                components.Add(comp.Expect());
            }

            foreach(var file in files)
            {
                var comp = registryManager.CreateComponentMetadata(new(file));
                components.Add(comp.Expect());
            }

            AnsiConsole.WriteLine("");
            AnsiConsole.WriteLine("Directories: " + JsonSerializer.Serialize(directories));
            AnsiConsole.WriteLine("");
            AnsiConsole.WriteLine("Files: " + JsonSerializer.Serialize(files));
            AnsiConsole.WriteLine("");
            AnsiConsole.WriteLine("Components: " + JsonSerializer.Serialize(components));
            AnsiConsole.WriteLine("");
        } */

        /*         if(path is SingleComponent searchAllComponents)
                {
                    AnsiConsole.WriteLine("Single: " + searchAllComponents.Path);  
                } */

        // if(settings.AllComponents ?? false)
        // {
        //     var components = AnsiConsole.Prompt(
        //     new MultiSelectionPrompt<string>()
        //         .Title("\nWhich [green]components would you like to register[/]? [gray](if none is selected, all components will be registered)[/]")
        //         .NotRequired() // Not required to have a favorite fruit
        //         .PageSize(10)
        //         .InstructionsText(
        //             "[grey](If you want to register all components, select none and press enter)[/]\n\n" +
        //             "[grey](Press [blue]<space>[/] to toggle a fruit, " + 
        //             "[green]<enter>[/] to accept)[/]\n")
        //         .AddChoices([..componentsJson.Select(c => ti.ToTitleCase(c.Name))]));

        //     if(components.Count == 0)
        //     {
        //         AnsiConsole.WriteLine("Register all components from json: " + JsonSerializer.Serialize(componentsJson));
        //     }
        //     else 
        //     {
        //         foreach (string component in components)
        //         {
        //             AnsiConsole.WriteLine("register: " + component);
        //         }
        //     }
        //     return 0;
        // }

        // AnsiConsole.MarkupLine($"\nComponent to search, [blue]{componentToSearch}[/]\n\nRegistry URI, {registryUri}");

        // var registryUriObj = new Uri(registryUri);
        // var registryManager = new RegistryManager(new(registryUriObj));

        // //AnsiConsole.MarkupLine($"Component Json: {componentJson.Expect()}");
        // AnsiConsole.WriteLine($"Component Json: {JsonSerializer.Serialize(componentJson.Expect())}");
        // return 0;
    }

    public override ValidationResult Validate(CommandContext context, Settings settings)
    {
        AnsiConsole.WriteLine();

        /*         if (string.IsNullOrEmpty(settings.ComponentsSource))
                {
                    return ValidationResult.Error($"You must pass a source (-s|--src) to find the component(s) to register | Example: `registry-create-metadata path.to.register/ -s ./UI/*`\n");
                } */

        if (string.IsNullOrEmpty(settings.RegistryUri))
        {
            return ValidationResult.Error($"You must pass a URI to register component(s) | Example: `registry-create-metadata path.to.register/ -s ./UI/*`\n");
        }
        return base.Validate(context, settings);
    }
}