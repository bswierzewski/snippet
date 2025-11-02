using Snippet.Modules.Snippets.Domain.Aggregates;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Domain.Entities;

/// <summary>
/// Represents the many-to-many relationship between a Snippet and a Collection.
/// This is an explicit join entity managed by EF Core.
/// </summary>
public class SnippetCollection
{
    /// <summary>
    /// Gets the identifier of the snippet.
    /// </summary>
    public SnippetId SnippetId { get; private set; }

    /// <summary>
    /// Gets the identifier of the collection.
    /// </summary>
    public CollectionId CollectionId { get; private set; }

    /// <summary>
    /// Navigation property to the snippet.
    /// </summary>
    public Aggregates.Snippet Snippet { get; private set; } = null!;

    /// <summary>
    /// Navigation property to the collection.
    /// </summary>
    public Collection Collection { get; private set; } = null!;

    // EF Core constructor
    private SnippetCollection() { }

    /// <summary>
    /// Creates a new snippet-collection relationship.
    /// </summary>
    public SnippetCollection(SnippetId snippetId, CollectionId collectionId)
    {
        SnippetId = snippetId;
        CollectionId = collectionId;
    }
}
