using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;

namespace Snippet.Modules.Snippets.Infrastructure.Persistence;

/// <summary>
/// Database context for Snippets module.
/// Implements both read and write interfaces for CQRS pattern.
/// </summary>
public sealed class SnippetsDbContext : DbContext, ISnippetsReadDbContext, ISnippetsWriteDbContext
{
    public SnippetsDbContext(DbContextOptions<SnippetsDbContext> options) : base(options)
    {
    }

    // DbSet properties will be added here as domain entities are created
    // Example: public DbSet<Snippet> Snippets => Set<Snippet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply entity configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SnippetsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
