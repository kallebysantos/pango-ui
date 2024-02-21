using Pango.Abstractions;
using System.Net.Http.Json;

namespace Pango.ServicesHttp;


//public record struct ComponentsJson(Option<string?> Name, Option<string?> Reference, Option<string?> Id);
public record struct ComponentsJson(string Name, string Reference);

public static class ServiceHttp
{
    public static async Task<List<ComponentsJson>> GetComponentsJsonAsync()
    {
        using HttpClient http = new();
        var obj = await http.GetFromJsonAsync<List<ComponentsJson>>("http://localhost:3000/components");
        
        if (obj is null)
        {
            return [];
        }

        return obj;
    } 
}
