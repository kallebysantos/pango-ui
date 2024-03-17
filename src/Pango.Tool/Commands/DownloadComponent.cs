using Spectre.Console.Cli;
using Pango.Abstractions;
using Pango.Services.RegistryClient;
using System.Diagnostics.CodeAnalysis;
using ComponentModel = System.ComponentModel;

public sealed class DownloadComponent : AsyncCommand<DownloadComponent.Settings>
{
    public sealed class Settings : CommandSettings
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

    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] Settings settings)
    {

        var httpClient = new HttpClient();

        var foundComponentResult = await Component.ResolveFrom(
            httpClient: httpClient,
            remoteUri: settings.RegistryUri,
            name: settings.ComponentName
        );


        if (foundComponentResult.Ok() is not Some<Component<Resolved>> component)
        {
            Console.WriteLine(foundComponentResult);
            Console.WriteLine("Error");
            return 1;
        }


        await foreach (var item in component.Value.GetComponentStreams(httpClient))
        {
            if (item.IsErr())
            {
                Console.WriteLine($"Error: {item}");
                continue;
            }

            var downloadResult = await item.Expect().Download(
                localBaseComponentPath: settings.Output,
                localBaseNamespace: settings.Namespace
            );

            downloadResult.Match(
                component => Console.WriteLine(component),
                err: Console.WriteLine
            );
        }

        return 0;
    }
}