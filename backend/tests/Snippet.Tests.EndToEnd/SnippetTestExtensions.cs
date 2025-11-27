using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Tests.Builders;
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
    /// Generates a test JWT token with specified claims.
    /// Uses JwtTokenBuilder to create a valid JWT structure.
    /// </summary>
    /// <param name="context">The test context.</param>
    /// <param name="email">Email claim (required)</param>
    /// <param name="userId">Subject (sub) claim - external user ID. Defaults to random GUID.</param>
    /// <param name="displayName">Display name claim. Optional.</param>
    /// <param name="additionalClaims">Additional custom claims. Optional.</param>
    /// <returns>A valid JWT token string</returns>
    public static string GenerateUserToken(
        this TestContext context,
        string email,
        string? userId = null,
        string? displayName = null,
        Dictionary<string, string>? additionalClaims = null)
    {
        var builder = new JwtTokenBuilder()
            .WithEmail(email)
            .WithSubject(userId ?? Guid.NewGuid().ToString());

        if (!string.IsNullOrEmpty(displayName))
            builder.WithDisplayName(displayName);

        if (additionalClaims != null)
            foreach (var claim in additionalClaims)
                builder.WithClaim(claim.Key, claim.Value);

        return builder.Build();
    }

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
}
