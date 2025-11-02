using BuildingBlocks.Domain.Primitives;
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
    /// <param name="collections">Optional collections to assign.</param>
    public Snippet(
        SnippetId id,
        Guid userId,
        string title,
        string content,
        ProgrammingLanguage language,
        string? description = null,
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
    /// Assigns a tag to the snippet.
    /// </summary>
    /// <param name="tag">Tag to assign.</param>
    public void AssignTag(Tag tag)
    {
        if (!_snippetTags.Any(st => st.TagId == tag.Id))
        {
            var snippetTag = new SnippetTag(Id, tag.Id);
            _snippetTags.Add(snippetTag);
        }
    }

    /// <summary>
    /// Removes a tag from the snippet.
    /// </summary>
    /// <param name="tag">Tag to remove.</param>
    public void RemoveTag(Tag tag)
    {
        _snippetTags.RemoveAll(st => st.TagId == tag.Id);
    }

    /// <summary>
    /// Adds the snippet to a collection.
    /// </summary>
    /// <param name="collection">Collection to add.</param>
    public void AddToCollection(Collection collection)
    {
        if (!_snippetCollections.Any(sc => sc.CollectionId == collection.Id))
        {
            var snippetCollection = new SnippetCollection(Id, collection.Id);
            _snippetCollections.Add(snippetCollection);
        }
    }

    /// <summary>
    /// Removes the snippet from a collection.
    /// </summary>
    /// <param name="collection">Collection to remove.</param>
    public void RemoveFromCollection(Collection collection)
    {
        _snippetCollections.RemoveAll(sc => sc.CollectionId == collection.Id);
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
    /// Removes the snippet from all collections.
    /// </summary>
    public void RemoveFromAllCollections()
    {
        _snippetCollections.Clear();
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
