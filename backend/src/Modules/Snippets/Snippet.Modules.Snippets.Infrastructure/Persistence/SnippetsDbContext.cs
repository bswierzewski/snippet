using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.Aggregates;

namespace Snippet.Modules.Snippets.Infrastructure.Persistence;

/// <summary>
/// Database context for Snippets module.
/// Implements both read and write interfaces for CQRS pattern.
/// </summary>
public sealed class SnippetsDbContext(DbContextOptions<SnippetsDbContext> options) : DbContext(options), ISnippetsReadDbContext, ISnippetsWriteDbContext
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

    // Explicit interface implementations for ISnippetsWriteDbContext
    DbSet<Domain.Aggregates.Snippet> ISnippetsWriteDbContext.Snippets => Snippets;
    DbSet<Collection> ISnippetsWriteDbContext.Collections => Collections;
    DbSet<Tag> ISnippetsWriteDbContext.Tags => Tags;

    // Explicit interface implementations for ISnippetsReadDbContext
    IQueryable<Domain.Aggregates.Snippet> ISnippetsReadDbContext.Snippets => Snippets.AsNoTracking();
    IQueryable<Collection> ISnippetsReadDbContext.Collections => Collections.AsNoTracking();
    IQueryable<Tag> ISnippetsReadDbContext.Tags => Tags.AsNoTracking();

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
