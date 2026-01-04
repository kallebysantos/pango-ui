using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazorSSR.Services;

public class ServerHostEnvironment(
    IWebHostEnvironment hostEnvironment,
    NavigationManager navigationManager)
    : IWebAssemblyHostEnvironment
{
    public string Environment { get; } = hostEnvironment.EnvironmentName;
    public string BaseAddress { get; } = navigationManager.BaseUri;
}
