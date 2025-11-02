using Snippet.Modules.Snippets.Domain.Aggregates;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Domain.Entities;

/// <summary>
/// Represents the many-to-many relationship between a Snippet and a Tag.
/// This is an explicit join entity managed by EF Core.
/// </summary>
public class SnippetTag
{
    /// <summary>
    /// Gets the identifier of the snippet.
    /// </summary>
    public SnippetId SnippetId { get; private set; }

    /// <summary>
    /// Gets the identifier of the tag.
    /// </summary>
    public TagId TagId { get; private set; }

    /// <summary>
    /// Navigation property to the snippet.
    /// </summary>
    public Aggregates.Snippet Snippet { get; private set; } = null!;

    /// <summary>
    /// Navigation property to the tag.
    /// </summary>
    public Tag Tag { get; private set; } = null!;

    // EF Core constructor
    private SnippetTag() { }

    /// <summary>
    /// Creates a new snippet-tag relationship.
    /// </summary>
    public SnippetTag(SnippetId snippetId, TagId tagId)
    {
        SnippetId = snippetId;
        TagId = tagId;
    }
}
