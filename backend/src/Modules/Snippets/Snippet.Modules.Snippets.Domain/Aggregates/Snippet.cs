using BuildingBlocks.Domain.Primitives;
using Snippet.Modules.Snippets.Domain.Enums;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Domain.Aggregates;

/// <summary>
/// Represents a code snippet aggregate root containing code, queries, or prompts with associated metadata and tags.
/// </summary>
public class Snippet : AggregateRoot<SnippetId>
{
    private readonly List<Tag> _tags = new();
    private readonly List<CollectionId> _collectionIds = new();

    /// <summary>
    /// Gets the identifier of the user who owns this snippet.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the title of the snippet.
    /// </summary>
    public string Title { get; private set; } = null!;

    /// <summary>
    /// Gets the description of the snippet.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the content of the snippet (code, query, or prompt text).
    /// </summary>
    public string Content { get; private set; } = null!;

    /// <summary>
    /// Gets the programming language for syntax highlighting.
    /// </summary>
    public ProgrammingLanguage Language { get; private set; }

    /// <summary>
    /// Gets the read-only collection of collection identifiers this snippet belongs to.
    /// </summary>
    public IReadOnlyList<CollectionId> CollectionIds => _collectionIds.AsReadOnly();

    /// <summary>
    /// Gets whether the snippet is marked as favorite.
    /// </summary>
    public bool IsFavorite { get; private set; }

    /// <summary>
    /// Gets the number of times this snippet has been used.
    /// </summary>
    public int UsageCount { get; private set; }

    /// <summary>
    /// Gets the date and time when the snippet was last used.
    /// </summary>
    public DateTimeOffset? LastUsedAt { get; private set; }

    /// <summary>
    /// Gets the read-only collection of tags associated with this snippet.
    /// </summary>
    public IReadOnlyList<Tag> Tags => _tags.AsReadOnly();

    private Snippet() { }

    /// <summary>
    /// Creates a new snippet with the specified details.
    /// </summary>
    /// <param name="id">Unique identifier for the snippet.</param>
    /// <param name="userId">Identifier of the user who owns the snippet.</param>
    /// <param name="title">Title of the snippet.</param>
    /// <param name="content">Content of the snippet.</param>
    /// <param name="language">Programming language for syntax highlighting.</param>
    /// <param name="description">Optional description of the snippet.</param>
    /// <param name="collectionIds">Optional collection identifiers.</param>
    public Snippet(
        SnippetId id,
        Guid userId,
        string title,
        string content,
        ProgrammingLanguage language,
        string? description = null,
        IEnumerable<CollectionId>? collectionIds = null)
    {
        Id = id;
        UserId = userId;
        Title = title;
        Description = description;
        Content = content;
        Language = language;
        IsFavorite = false;
        UsageCount = 0;
        LastUsedAt = null;

        if (collectionIds is not null)
            _collectionIds.AddRange(collectionIds);
    }

    /// <summary>
    /// Updates the snippet's content.
    /// </summary>
    /// <param name="content">New content for the snippet.</param>
    public void UpdateContent(string content)
    {
        Content = content;
    }

    /// <summary>
    /// Updates the snippet's title and description.
    /// </summary>
    /// <param name="title">New title for the snippet.</param>
    /// <param name="description">New description for the snippet.</param>
    public void UpdateDetails(string title, string? description)
    {
        Title = title;
        Description = description;
    }

    /// <summary>
    /// Changes the programming language of the snippet.
    /// </summary>
    /// <param name="language">New programming language.</param>
    public void ChangeLanguage(ProgrammingLanguage language)
    {
        Language = language;
    }

    /// <summary>
    /// Adds a tag to the snippet.
    /// </summary>
    /// <param name="tag">Tag to add.</param>
    public void AddTag(Tag tag)
    {
        if (_tags.Any(t => t.Name.Equals(tag.Name, StringComparison.OrdinalIgnoreCase)))
            return;

        _tags.Add(tag);
    }

    /// <summary>
    /// Removes a tag from the snippet by its identifier.
    /// </summary>
    /// <param name="tagId">Identifier of the tag to remove.</param>
    public void RemoveTag(TagId tagId)
    {
        var tag = _tags.FirstOrDefault(t => t.Id == tagId);
        if (tag != null)
            _tags.Remove(tag);
    }

    /// <summary>
    /// Adds the snippet to a collection.
    /// </summary>
    /// <param name="collectionId">Collection identifier to add.</param>
    public void AddToCollection(CollectionId collectionId)
    {
        if (!_collectionIds.Contains(collectionId))
            _collectionIds.Add(collectionId);
    }

    /// <summary>
    /// Removes the snippet from a collection.
    /// </summary>
    /// <param name="collectionId">Collection identifier to remove.</param>
    public void RemoveFromCollection(CollectionId collectionId)
    {
        _collectionIds.Remove(collectionId);
    }

    /// <summary>
    /// Updates the snippet's collection assignments.
    /// </summary>
    /// <param name="collectionIds">New collection identifiers.</param>
    public void UpdateCollections(IEnumerable<CollectionId> collectionIds)
    {
        _collectionIds.Clear();
        _collectionIds.AddRange(collectionIds);
    }

    /// <summary>
    /// Removes the snippet from all collections.
    /// </summary>
    public void RemoveFromAllCollections()
    {
        _collectionIds.Clear();
    }

    /// <summary>
    /// Toggles the favorite status of the snippet.
    /// </summary>
    public void ToggleFavorite()
    {
        IsFavorite = !IsFavorite;
    }

    /// <summary>
    /// Records usage of the snippet by incrementing the usage count and updating the last used timestamp.
    /// </summary>
    public void RecordUsage()
    {
        UsageCount++;
        LastUsedAt = DateTimeOffset.UtcNow;
    }
}
