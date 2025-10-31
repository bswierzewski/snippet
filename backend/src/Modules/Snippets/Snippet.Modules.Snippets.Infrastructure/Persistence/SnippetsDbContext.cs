using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.Aggregates;

namespace Snippet.Modules.Snippets.Infrastructure.Persistence;

/// <summary>
/// Database context for Snippets module.
/// Implements both read and write interfaces for CQRS pattern.
/// </summary>
public sealed class SnippetsDbContext : DbContext, ISnippetsReadDbContext, ISnippetsWriteDbContext
{
    /// <summary>
    /// Gets or sets the collection of snippets.
    /// </summary>
    public DbSet<Domain.Aggregates.Snippet> Snippets => Set<Domain.Aggregates.Snippet>();

    /// <summary>
    /// Gets or sets the collection of collections.
    /// </summary>
    public DbSet<Collection> Collections => Set<Collection>();

    // Explicit interface implementations for ISnippetsWriteDbContext
    DbSet<Domain.Aggregates.Snippet> ISnippetsWriteDbContext.Snippets => Snippets;
    DbSet<Collection> ISnippetsWriteDbContext.Collections => Collections;

    // Explicit interface implementations for ISnippetsReadDbContext
    IQueryable<Domain.Aggregates.Snippet> ISnippetsReadDbContext.Snippets => Snippets.AsNoTracking();
    IQueryable<Collection> ISnippetsReadDbContext.Collections => Collections.AsNoTracking();
    
    public SnippetsDbContext(DbContextOptions<SnippetsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SnippetsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
