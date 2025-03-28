using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Pango.UI.Components;

namespace Pango.UI.Shared.Docs;

public interface IMdxFragment
{
    void Render(RenderTreeBuilder builder);
    int GetIndex();
}

public record struct MdxFragment(int Index, string Content) : IMdxFragment
{
    public readonly int GetIndex() => Index;

    public readonly void Render(RenderTreeBuilder builder)
    {
        builder.AddMarkupContent(0, Content);
    }
}

public record struct MdxComponentFragment(
    int Index,
    string? Content,
    Type Type,
    IEnumerable<KeyValuePair<string, object>>? Attributes
) : IMdxFragment
{
    public readonly int GetIndex() => Index;

    public readonly void Render(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(2, "class", "markdown-component my-4");

        builder.OpenComponent(3, Type);
        builder.AddMultipleAttributes(4, Attributes);

        if (!string.IsNullOrEmpty(Content))
        {
            var content = string.Intern(Content);
            RenderFragment renderContent = b =>
            {
                b.AddContent(6, content);
            };
            builder.AddComponentParameter(5, "ChildContent", renderContent);
        }

        builder.CloseComponent();
        builder.CloseElement();
    }
}

public class MdxRenderer : ComponentBase
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
        _processedHtml = ParseMarkdown(Value ?? "");
    }

    public string ParseMarkdown(string markdown)
    {
        // Convert Markdown to HTML
        string html = Markdown.ToHtml(markdown, _pipeline);

        return html;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        Regex ComponentRegex = new(
            //@"<([A-Z][A-Za-z0-9]*)(\s+[^>]*)?>.*?</\1>|<([A-Z][A-Za-z0-9]*)(\s+[^>]*)?/?>",
            @"<(?<name>[A-Z][A-Za-z0-9]*)(?<props>\s+[^>]*)?(?:>(?<content>.*?)</\k<name>>|/?>)",
            RegexOptions.Compiled
        );

        List<IMdxFragment> _htmlChunks = [];
        int lastIndex = 0;

        var matches = ComponentRegex.Matches(_processedHtml);
        Console.WriteLine($"MATCHES: {matches.Count}");
        foreach (Match component in matches)
        {
            // Store HTML before the component
            var beforePlaceholder = _processedHtml[lastIndex..component.Index];
            _htmlChunks.Add(new MdxFragment(Index: lastIndex, Content: beforePlaceholder));

            var componentName =
                component
                    .Groups.Values.Where(g => g.Name == "name")
                    .Select(g => g.Captures.FirstOrDefault()?.Value)
                    .FirstOrDefault() ?? throw new Exception("Invalid componentName");

            var componentType =
                Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.IsClass && t.Name == componentName)
                    .FirstOrDefault() ?? throw new Exception("Could not find implementation type");

            var componentContent = component
                .Groups.Values.Where(g => g.Name == "content")
                .SelectMany(g => g.Captures.Where(c => !string.IsNullOrEmpty(c.Value)))
                .FirstOrDefault()
                ?.Value;

            Console.WriteLine(
                $"NAME: {componentName}, TYPE: {componentType}, Content: {componentContent}"
            );

            _htmlChunks.Add(
                new MdxComponentFragment(
                    Index: component.Index,
                    Content: componentContent,
                    Type: componentType!,
                    null
                )
            );

            lastIndex = component.Index + component.Length;

            /*
            foreach (var group in component.Groups.Values)
            {
                Console.WriteLine($"GROUP: {group.Name}: {group.Value}");

            }
            */
        }

        // Render Markdown content as raw HTML
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "markdown-body");

        int seq = 2;
        foreach (var fragment in _htmlChunks)
        {
            seq += fragment.GetIndex() + 1;

            Console.WriteLine($"Render at {seq}");
            builder.OpenRegion(seq);
            fragment.Render(builder);
            builder.CloseRegion();
        }

        builder.CloseElement();
    }
}
