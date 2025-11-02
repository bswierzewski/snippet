using BuildingBlocks.Domain.Primitives;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Domain.Aggregates;

/// <summary>
/// Represents a tag aggregate root that categorizes and labels snippets for organization and filtering.
/// Tags are shared across snippets and belong to a user.
/// </summary>
public class Tag : AggregateRoot<TagId>
{
    /// <summary>
    /// Gets the identifier of the user who owns this tag.
    /// </summary>
    public Guid UserId { get; private set; }

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
    /// <param name="userId">Identifier of the user who owns the tag.</param>
    /// <param name="name">Name of the tag.</param>
    /// <param name="color">Optional color in hexadecimal format.</param>
    public Tag(TagId id, Guid userId, string name, string? color = null)
    {
        Id = id;
        UserId = userId;
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
