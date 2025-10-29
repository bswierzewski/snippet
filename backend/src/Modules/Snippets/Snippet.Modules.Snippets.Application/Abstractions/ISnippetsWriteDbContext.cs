namespace Snippet.Modules.Snippets.Application.Abstractions;

/// <summary>
/// Write database context interface for modifying snippets data.
/// </summary>
public interface ISnippetsWriteDbContext
{
    // DbSet properties will be added here as domain entities are created
    // Example: DbSet<Snippet> Snippets { get; }

    /// <summary>
    /// Saves all pending changes to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation</param>
    /// <returns>The number of entities written to the database</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
