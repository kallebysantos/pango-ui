using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.RegularExpressions;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Pango.UI.Shared.Docs;

public interface IMdxFragment
{
    void Render(RenderTreeBuilder builder);
}

public record struct MdxFragment(string Content) : IMdxFragment
{
    public readonly void Render(RenderTreeBuilder builder)
    {
        builder.AddMarkupContent(0, Content);
    }
}

public record struct MdxComponentFragment(
    string? Content,
    Type Type,
    IEnumerable<KeyValuePair<string, object>>? Attributes,
    IEnumerable<KeyValuePair<string, object>>? Parameters
) : IMdxFragment
{
    public readonly void Render(RenderTreeBuilder builder)
    {
        builder.OpenComponent(0, Type);
        builder.AddMultipleAttributes(1, Attributes);

        if (!string.IsNullOrEmpty(Content))
        {
            var content = string.Intern(Content);
            int seq = 3;
            RenderFragment renderChild = childBuilder =>
            {
                var fragments = MdxRenderer.ResolveComponent(content);

                childBuilder.OpenRegion(seq);
                foreach (var (index, fragment) in fragments)
                {
                    seq += index + 1;
                    fragment.Render(childBuilder);
                }
                childBuilder.CloseRegion();
            };

            builder.AddComponentParameter(seq, "ChildContent", renderChild);
        }

        if (Parameters is not null)
        {
            int seq = 1000;
            foreach (var (key, value) in Parameters)
            {
                var property = Type.GetProperties().FirstOrDefault(p => p.Name == key);
                if (property is null)
                    continue;
                //
                // Clean the value string if it's wrapped in "@(...)"
                var stringValue = value?.ToString()?.Trim();
                if (stringValue is null)
                    continue;

                if (stringValue.StartsWith("@(") && stringValue.EndsWith(")"))
                {
                    // Remove the "@(" prefix and the closing ")"
                    stringValue = stringValue[2..^1].Trim();
                }

                var parsed =
                    (property.PropertyType == typeof(string))
                        ? stringValue
                        : JsonSerializer.Deserialize(stringValue, property.PropertyType);

                builder.AddComponentParameter(seq++, key, parsed);
            }
        }

        builder.CloseComponent();
    }
}

public partial class MdxRenderer : ComponentBase
{
    [Parameter]
    public string? Value { get; set; }

    private readonly MarkdownPipeline _pipeline;

    private string _processedHtml = "";

    public MdxRenderer()
    {
        _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
    }

    protected override void OnParametersSet()
    {
        if (string.IsNullOrEmpty(Value))
            return;

        // Process Markdown
        _processedHtml = Markdown.ToHtml(Value, _pipeline);
        ;
    }

    public static IEnumerable<(int, IMdxFragment)> ResolveComponent(string html)
    {
        List<(int, IMdxFragment)> fragments = [];

        var matches = RazorComponentPattern().Matches(html);

        int lastIndex = 0;
        foreach (Match component in matches)
        {
            // Store HTML before the component
            var beforePlaceholder = html[lastIndex..component.Index];
            fragments.Add((lastIndex, new MdxFragment(Content: beforePlaceholder)));

            var componentName =
                component
                    .Groups.Values.Where(g => g.Name == "name")
                    .Select(g => g.Captures.FirstOrDefault()?.Value)
                    .FirstOrDefault() ?? throw new Exception("Invalid componentName");

            var componentType =
                AppDomain
                    .CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.IsClass && t.Name == componentName)
                    .FirstOrDefault() ?? throw new Exception("Could not find implementation type");

            var componentContent = component
                .Groups.Values.Where(g => g.Name == "content")
                .SelectMany(g => g.Captures.Where(c => !string.IsNullOrEmpty(c.Value)))
                .FirstOrDefault()
                ?.Value;

            var props = component.Groups["props"].Value;
            List<KeyValuePair<string, object>> attributes = [];
            List<KeyValuePair<string, object>> parameters = [];

            if (!string.IsNullOrWhiteSpace(props))
            {
                // Use a regex to match attribute key/value pairs.
                // This regex supports both double and single quotes.
                var attributeRegex = RazorAttributePattern();

                foreach (Match attribute in attributeRegex.Matches(props))
                {
                    var key = attribute.Groups["key"].Value;
                    var value = attribute.Groups["value"].Value;
                    attributes.Add(new(key, value));
                }

                var parameterRegex = RazorParameterPattern();

                foreach (Match parameter in parameterRegex.Matches(props))
                {
                    var key = parameter.Groups["key"].Value;
                    var value = parameter.Groups["value"].Value;
                    Console.WriteLine($"PARAM: {key} - {value}");
                    parameters.Add(new(key, value));
                }
            }

            fragments.Add(
                (
                    component.Index,
                    new MdxComponentFragment(
                        Content: componentContent,
                        Type: componentType!,
                        attributes,
                        parameters
                    )
                )
            );

            lastIndex = component.Index + component.Length;
        }

        fragments.Add((lastIndex, new MdxFragment(html[lastIndex..])));

        return fragments;
    }

    protected void RenderMarkdown(RenderTreeBuilder builder)
    {
        var fragments = ResolveComponent(_processedHtml);

        // Render Markdown content as raw HTML
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "markdown-body");

        int seq = 2;
        foreach (var (index, fragment) in fragments)
        {
            seq += index + 1;

            builder.OpenRegion(seq);

            if (fragment is MdxComponentFragment)
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "class", "markdown-component");
            }

            fragment.Render(builder);

            if (fragment is MdxComponentFragment)
            {
                builder.CloseElement();
            }

            builder.CloseRegion();
        }

        builder.CloseElement();
    }

    [GeneratedRegex(
        @"(?s)<(?<name>[A-Z][A-Za-z0-9]*)(?<props>\s+[^>]*)?(?:>(?<content>.*?)</\k<name>>|/?>)",
        RegexOptions.Compiled
    )]
    private static partial Regex RazorComponentPattern();

    [GeneratedRegex(
        @"(?<=\s|^)(?<key>[a-z][a-z0-9-]*)(?:\s*=\s*""(?<value>[^""]*)"")?",
        RegexOptions.Compiled
    )]
    private static partial Regex RazorAttributePattern();

    [GeneratedRegex(
        @"(?<=\s|^)(?<key>[A-Z][A-Za-z0-9]*)(?:\s*=\s*(?:""(?<value>[^""]*)""|(?<value>@\([^)]*\))))?",
        RegexOptions.Compiled
    )]
    private static partial Regex RazorParameterPattern();
}
