using BuildingBlocks.Domain.Primitives;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Domain.Aggregates;

/// <summary>
/// Represents a tag entity that categorizes and labels snippets for organization and filtering.
/// </summary>
public class Tag : Entity<TagId>
{
    /// <summary>
    /// Gets the identifier of the snippet this tag belongs to.
    /// </summary>
    public SnippetId SnippetId { get; private set; } = null!;

    /// <summary>
    /// Gets the name of the tag.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Gets the color of the tag in hexadecimal format (e.g., #FF5733).
    /// </summary>
    public string? Color { get; private set; }

    private Tag() { }

    /// <summary>
    /// Creates a new tag with the specified details.
    /// </summary>
    /// <param name="id">Unique identifier for the tag.</param>
    /// <param name="snippetId">Identifier of the snippet this tag belongs to.</param>
    /// <param name="name">Name of the tag.</param>
    /// <param name="color">Optional color in hexadecimal format.</param>
    public Tag(TagId id, SnippetId snippetId, string name, string? color = null)
    {
        Id = id;
        SnippetId = snippetId;
        Name = name;
        Color = color;
    }

    /// <summary>
    /// Updates the tag's name.
    /// </summary>
    /// <param name="name">New name for the tag.</param>
    public void Rename(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Updates the tag's color.
    /// </summary>
    /// <param name="color">New color in hexadecimal format.</param>
    public void ChangeColor(string? color)
    {
        Color = color;
    }
}
