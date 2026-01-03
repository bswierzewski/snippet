using BuildingBlocks.Abstractions.Primitives;
using Snippet.Modules.Snippets.Domain.Entities;
using Snippet.Modules.Snippets.Domain.Enums;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Domain.Aggregates;

/// <summary>
/// Represents a code snippet aggregate root containing code, queries, or prompts with associated metadata and tags.
/// </summary>
public class Snippet : AggregateRoot<SnippetId>
{
    private readonly List<SnippetTag> _snippetTags = [];
    private readonly List<SnippetCollection> _snippetCollections = [];

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
    /// Gets the read-only collection of snippet-tag relationships.
    /// </summary>
    public IReadOnlyCollection<SnippetTag> SnippetTags => _snippetTags.AsReadOnly();

    /// <summary>
    /// Gets the read-only collection of snippet-collection relationships.
    /// </summary>
    public IReadOnlyCollection<SnippetCollection> SnippetCollections => _snippetCollections.AsReadOnly();

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
    /// <param name="tags">Optional tags to assign.</param>
    /// <param name="collections">Optional collections to assign.</param>
    public Snippet(
        SnippetId id,
        Guid userId,
        string title,
        string content,
        ProgrammingLanguage language,
        string? description = null,
        IEnumerable<Tag>? tags = null,
        IEnumerable<Collection>? collections = null)
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

        if (tags is not null)
        {
            foreach (var tag in tags)
            {
                var snippetTag = new SnippetTag(id, tag.Id);
                _snippetTags.Add(snippetTag);
            }
        }

        if (collections is not null)
        {
            foreach (var collection in collections)
            {
                var snippetCollection = new SnippetCollection(id, collection.Id);
                _snippetCollections.Add(snippetCollection);
            }
        }
    }

    /// <summary>
    /// Updates the snippet with new data.
    /// </summary>
    /// <param name="title">New title for the snippet.</param>
    /// <param name="description">New description for the snippet.</param>
    /// <param name="content">New content for the snippet.</param>
    /// <param name="language">New programming language.</param>
    public void Update(string title, string? description, string content, ProgrammingLanguage language)
    {
        Title = title;
        Description = description;
        Content = content;
        Language = language;
    }

    /// <summary>
    /// Updates the snippet's collection assignments.
    /// </summary>
    /// <param name="collections">New collections.</param>
    public void UpdateCollections(IEnumerable<Collection> collections)
    {
        _snippetCollections.Clear();
        foreach (var collection in collections)
        {
            var snippetCollection = new SnippetCollection(Id, collection.Id);
            _snippetCollections.Add(snippetCollection);
        }
    }

    /// <summary>
    /// Updates the snippet's tag assignments.
    /// </summary>
    /// <param name="tags">New tags.</param>
    public void UpdateTags(IEnumerable<Tag> tags)
    {
        _snippetTags.Clear();
        foreach (var tag in tags)
        {
            var snippetTag = new SnippetTag(Id, tag.Id);
            _snippetTags.Add(snippetTag);
        }
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
