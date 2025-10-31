using Snippet.Modules.Snippets.Domain.Aggregates;

namespace Snippet.Modules.Snippets.Application.Abstractions;

/// <summary>
/// Read-only database context interface for querying snippets data.
/// </summary>
public interface ISnippetsReadDbContext
{
    /// <summary>
    /// Gets the queryable collection of snippets for read operations.
    /// </summary>
    IQueryable<Domain.Aggregates.Snippet> Snippets { get; }

    /// <summary>
    /// Gets the queryable collection of collections for read operations.
    /// </summary>
    IQueryable<Collection> Collections { get; }
}
