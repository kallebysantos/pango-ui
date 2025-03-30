using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using TailwindMerge.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder
    .Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddDirectoryBrowser();

// Add TailwindMerge support
builder.Services.AddTailwindMerge();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.WebHost.GetSetting("urls") ?? string.Empty),
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Alternatively, use AppDomain.CurrentDomain.BaseDirectory
string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "Docs");

var fileProvider = new PhysicalFileProvider(baseDir);
var requestPath = "/Docs";

// Create content type provider and add .razor and .mdx mappings
var contentTypeProvider = new FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".razor"] = "text/plain"; // Serve .razor as plain text
contentTypeProvider.Mappings[".mdx"] = "text/markdown"; // Serve .mdx as markdown

// Enable displaying browser links.
app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = fileProvider,
        RequestPath = requestPath,
        ContentTypeProvider = contentTypeProvider,
    }
);

if (builder.Environment.IsDevelopment())
{
    app.UseDirectoryBrowser(
        new DirectoryBrowserOptions { FileProvider = fileProvider, RequestPath = requestPath }
    );
}
app.UseHttpsRedirection();

app.UseFileServer(enableDirectoryBrowsing: true);

app.UseAntiforgery();

app.MapRazorComponents<BlazorSSR.App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Pango.UI._Imports).Assembly);

app.Run();
