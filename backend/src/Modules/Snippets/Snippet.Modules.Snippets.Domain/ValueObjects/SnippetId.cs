namespace Snippet.Modules.Snippets.Domain.ValueObjects;

/// <summary>
/// Represents a unique identifier for a Snippet entity.
/// </summary>
/// <param name="Value">The unique identifier value.</param>
public record SnippetId(Guid Value);
