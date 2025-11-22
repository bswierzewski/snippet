using Shared.Abstractions.Primitives;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Domain.Aggregates;

/// <summary>
/// Represents a collection (group) aggregate root for organizing snippets.
/// </summary>
public class Collection : AggregateRoot<CollectionId>
{
    /// <summary>
    /// Gets the identifier of the user who owns this collection.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the name of the collection.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Gets the description of the collection.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the color of the collection in hexadecimal format (e.g., #3B82F6).
    /// </summary>
    public string? Color { get; private set; }

    /// <summary>
    /// Gets the icon name or emoji for the collection.
    /// </summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// Gets the sort order for displaying collections.
    /// </summary>
    public int SortOrder { get; private set; }

    private Collection() { }

    /// <summary>
    /// Creates a new collection with the specified details.
    /// </summary>
    /// <param name="id">Unique identifier for the collection.</param>
    /// <param name="userId">Identifier of the user who owns the collection.</param>
    /// <param name="name">Name of the collection.</param>
    /// <param name="description">Optional description of the collection.</param>
    /// <param name="color">Optional color in hexadecimal format.</param>
    /// <param name="icon">Optional icon name or emoji.</param>
    /// <param name="sortOrder">Sort order for display.</param>
    public Collection(
        CollectionId id,
        Guid userId,
        string name,
        string? description = null,
        string? color = null,
        string? icon = null,
        int sortOrder = 0)
    {
        Id = id;
        UserId = userId;
        Name = name;
        Description = description;
        Color = color;
        Icon = icon;
        SortOrder = sortOrder;
    }

    /// <summary>
    /// Renames the collection.
    /// </summary>
    /// <param name="name">New name for the collection.</param>
    public void Rename(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Updates the collection's description.
    /// </summary>
    /// <param name="description">New description for the collection.</param>
    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    /// <summary>
    /// Updates the collection's visual appearance (color and icon).
    /// </summary>
    /// <param name="color">New color in hexadecimal format.</param>
    /// <param name="icon">New icon name or emoji.</param>
    public void UpdateAppearance(string? color, string? icon)
    {
        Color = color;
        Icon = icon;
    }

    /// <summary>
    /// Updates the sort order of the collection.
    /// </summary>
    /// <param name="sortOrder">New sort order value.</param>
    public void Reorder(int sortOrder)
    {
        SortOrder = sortOrder;
    }
}
