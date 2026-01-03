using BuildingBlocks.Tests.Core;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using SnippetAggregate = Snippet.Modules.Snippets.Domain.Aggregates.Snippet;

namespace Snippet.Tests.EndToEnd;

/// <summary>
/// Extension methods for Snippet module tests.
/// Provides domain-specific test helpers as extension methods on TestContext.
/// </summary>
public static class SnippetTestExtensions
{
    /// <summary>
    /// Retrieves a snippet from the database by ID.
    /// </summary>
    /// <param name="context">The test context.</param>
    /// <param name="id">Snippet ID to search for</param>
    /// <returns>The snippet entity or throws if not found</returns>
    public static async Task<SnippetAggregate?> GetSnippetFromDbAsync(
        this TestContext context,
        Guid id)
    {
        var db = context.GetRequiredService<ISnippetDbContext>();
        return await db.Snippets
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id.Value == id);
    }

    /// <summary>
    /// Retrieves all snippets from the database.
    /// </summary>
    public static async Task<List<SnippetAggregate>> GetAllSnippetsFromDbAsync(
        this TestContext context)
    {
        var db = context.GetRequiredService<ISnippetDbContext>();
        return await db.Snippets
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Checks if a snippet exists in the database.
    /// </summary>
    public static async Task<bool> SnippetExistsAsync(
        this TestContext context,
        Guid id)
    {
        var db = context.GetRequiredService<ISnippetDbContext>();
        return await db.Snippets.AnyAsync(s => s.Id.Value == id);
    }
}
