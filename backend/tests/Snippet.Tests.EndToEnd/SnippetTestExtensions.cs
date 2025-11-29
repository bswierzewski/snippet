using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Tests.Authentication;
using Shared.Infrastructure.Tests.Core;
using Snippet.Modules.Snippets.Infrastructure.Persistence;
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
        var db = context.GetRequiredService<SnippetsDbContext>();
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
        var db = context.GetRequiredService<SnippetsDbContext>();
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
        var db = context.GetRequiredService<SnippetsDbContext>();
        return await db.Snippets.AnyAsync(s => s.Id.Value == id);
    }

    /// <summary>
    /// Generates a test JWT token for the specified user credentials.
    /// Wrapper around GetTokenAsync for simplified test token generation.
    /// </summary>
    /// <param name="context">The test context.</param>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    /// <returns>JWT token string</returns>
    public static async Task<string> GenerateUserToken(
        this TestContext context,
        string email,
        string password)
    {
        return await context.GetTokenAsync(email, password);
    }
}
