using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.Aggregates;

namespace Snippet.Modules.Snippets.Infrastructure.Persistence;

/// <summary>
/// Database context for Snippets module.
/// </summary>
public sealed class SnippetsDbContext(DbContextOptions<SnippetsDbContext> options) : DbContext(options), ISnippetDbContext
{
    /// <summary>
    /// Gets or sets the collection of snippets.
    /// </summary>
    public DbSet<Domain.Aggregates.Snippet> Snippets => Set<Domain.Aggregates.Snippet>();

    /// <summary>
    /// Gets or sets the collection of collections.
    /// </summary>
    public DbSet<Collection> Collections => Set<Collection>();

    /// <summary>
    /// Gets or sets the collection of tags.
    /// </summary>
    public DbSet<Tag> Tags => Set<Tag>();

    /// <summary>
    /// Configures the model and relationships for the Snippets module entities.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SnippetsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
