using System.Text.Json;
using System.Text.RegularExpressions;

namespace Pango.UI.Shared.Docs;

public record DocumentSectionRef
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public DocumentSectionRef[] Sections { get; set; } = [];
}

// Class to represent your document metadata
public record DocumentMetadata
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool Featured { get; set; }
    public bool IsComponent { get; set; }
    public string[] Tags { get; set; } = [];
    public DocumentSectionRef[] Sections { get; set; } = [];
}

public partial class MdxMetadata
{
    /// <summary>
    /// Parses the markdown file and extracts document metadata
    /// </summary>
    /// <param name="markdownContent">The content of the markdown file</param>
    /// <returns>Parsed DocumentMetadata or null if not found</returns>
    public static DocumentMetadata? ExtractMetadata(string markdownContent)
    {
        if (string.IsNullOrEmpty(markdownContent))
            return null;

        // Extract the JSON metadata section
        var match = MetadataSectionPattern().Match(markdownContent);
        if (!match.Success || match.Groups.Count < 2)
            return null;

        string jsonContent = match.Groups[1].Value.Trim();

        try
        {
            // Deserialize the JSON to DocumentMetadata class
            var metadata = JsonSerializer.Deserialize<DocumentMetadata>(
                jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return metadata;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to parse metadata JSON: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Removes the metadata section from markdown content
    /// </summary>
    /// <param name="markdownContent">The original markdown content with metadata</param>
    /// <returns>Clean markdown without the metadata section</returns>
    public static string RemoveMetadataSection(string markdownContent)
    {
        if (string.IsNullOrEmpty(markdownContent))
            return string.Empty;

        // Replace the metadata section with an empty string
        string cleanMarkdown = MetadataSectionPattern().Replace(markdownContent, string.Empty);

        // Trim any leading whitespace that might be left
        return cleanMarkdown.TrimStart();
    }

    /// Pattern to match the JSON metadata section between
    /// ```json:document-metadata and ```
    [GeneratedRegex(
        @"```json\s*:pango-ui-document-metadata\s*(.*?)\s*```",
        RegexOptions.Compiled | RegexOptions.Singleline
    )]
    private static partial Regex MetadataSectionPattern();
}
