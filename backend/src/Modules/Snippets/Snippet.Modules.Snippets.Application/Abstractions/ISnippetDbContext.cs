using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Domain.Aggregates;

namespace Snippet.Modules.Snippets.Application.Abstractions;

/// <summary>
/// Database context interface for snippets data operations.
/// </summary>
public interface ISnippetDbContext
{
    /// <summary>
    /// Gets the collection of snippets.
    /// </summary>
    DbSet<Domain.Aggregates.Snippet> Snippets { get; }

    /// <summary>
    /// Gets the collection of collections.
    /// </summary>
    DbSet<Collection> Collections { get; }

    /// <summary>
    /// Gets the collection of tags.
    /// </summary>
    DbSet<Tag> Tags { get; }

    /// <summary>
    /// Saves all pending changes to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The number of entities written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
