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
    IEnumerable<KeyValuePair<string, object>>? Attributes
) : IMdxFragment
{
    public readonly void Render(RenderTreeBuilder builder)
    {
        builder.OpenComponent(0, Type);
        builder.AddMultipleAttributes(1, Attributes);

        if (!string.IsNullOrEmpty(Content))
        {
            var content = string.Intern(Content);
            RenderFragment renderChild = childBuilder =>
            {
                var fragments = MdxRenderer.ResolveComponent(content);

                int seq = 4;
                childBuilder.OpenRegion(seq);
                foreach (var (index, fragment) in fragments)
                {
                    seq += index + 1;
                    fragment.Render(childBuilder);
                }
                childBuilder.CloseRegion();
            };

            builder.AddComponentParameter(3, "ChildContent", renderChild);
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

            fragments.Add(
                (
                    component.Index,
                    new MdxComponentFragment(Content: componentContent, Type: componentType!, null)
                )
            );

            lastIndex = component.Index + component.Length;
        }

        fragments.Add((lastIndex, new MdxFragment(html.Substring(lastIndex))));

        return fragments;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
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
                builder.AddAttribute(1, "class", "markdown-component my-4");
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
        @"<(?<name>[A-Z][A-Za-z0-9]*)(?<props>\s+[^>]*)?(?:>(?<content>.*?)</\k<name>>|/?>)",
        RegexOptions.Compiled
    )]
    private static partial Regex RazorComponentPattern();
}
