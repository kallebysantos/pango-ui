using System;
using System.Collections.Generic;
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
    public readonly int GetIndex() => Index + Content.Length;

    public readonly void Render(RenderTreeBuilder builder)
    {
        builder.AddMarkupContent(0, Content);
    }
}

public record struct MdxComponentFragment(
    int Index,
    string Content,
    Type Type,
    IEnumerable<KeyValuePair<string, object>>? Attributes
) : IMdxFragment
{
    public readonly int GetIndex() => Index + Content.Length;

    public readonly void Render(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(2, "class", "markdown-component");

        builder.OpenComponent(3, Type);
        builder.AddMultipleAttributes(4, Attributes);
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
    private static int _counter = 0;

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
        Regex ComponentRegex = new(@"<Razor\b[^>]*?/>", RegexOptions.Compiled);

        List<IMdxFragment> _htmlChunks = [];
        int lastIndex = 0;
        foreach (Match match in ComponentRegex.Matches(_processedHtml))
        {
            // Store HTML before the component
            var beforePlaceholder = _processedHtml[lastIndex..match.Index];
            _htmlChunks.Add(new MdxFragment(Index: lastIndex, Content: beforePlaceholder));

            Console.WriteLine($"COMPONENT: {match.Value}");

            // Store the component
            Regex attributesRegex = new(@"\b([a-zA-Z0-9-]+)\s*=\s*""([^""]*)");
            var a = attributesRegex.Match(match.Value);
            var componentType = Type.GetType(a.Value.Split("\"").Last());
            _htmlChunks.Add(
                new MdxComponentFragment(
                    Index: match.Index,
                    Content: match.Value,
                    Type: componentType!,
                    null
                )
            );

            Console.WriteLine($"\nBEFORE:{beforePlaceholder}\nCOMP:{a.Value}\n\n");

            lastIndex = match.Index + match.Length;
        }
        // Add the remaining HTML after the last component
        _htmlChunks.Add(
            new MdxFragment(Index: lastIndex, Content: _processedHtml.Substring(lastIndex))
        );

        // Render Markdown content as raw HTML
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "markdown-body");

        int seq = 2;
        foreach (var fragment in _htmlChunks)
        {
            seq += fragment.GetIndex() + 1;

            builder.OpenRegion(seq);
            fragment.Render(builder);
            builder.CloseRegion();
        }

        builder.CloseElement();

        // Render detected Blazor components at the correct positions
        /*
        Type? componentType = Type.GetType($"BlazorMarkdown.Components.Counter");

        if (componentType != null)
        {
            builder.OpenElement(3, "div");
            builder.OpenComponent(5, componentType);
            builder.CloseComponent();
            builder.CloseElement();
        }
        */
    }
}
