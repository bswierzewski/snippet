using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snippet.Modules.Snippets.Domain.Entities;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Infrastructure.Persistence.Configuration;

/// <summary>
/// Entity Framework configuration for the SnippetCollection join entity.
/// </summary>
public class SnippetCollectionConfiguration : IEntityTypeConfiguration<SnippetCollection>
{
    public void Configure(EntityTypeBuilder<SnippetCollection> builder)
    {
        builder.ToTable("SnippetCollections");

        // Composite primary key
        builder.HasKey(sc => new { sc.SnippetId, sc.CollectionId });

        // Convert SnippetId value object to Guid
        builder.Property(sc => sc.SnippetId)
            .HasConversion(
                id => id.Value,
                value => new SnippetId(value))
            .IsRequired();

        // Convert CollectionId value object to Guid
        builder.Property(sc => sc.CollectionId)
            .HasConversion(
                id => id.Value,
                value => new CollectionId(value))
            .IsRequired();

        // Configure relationship to Snippet
        builder.HasOne(sc => sc.Snippet)
            .WithMany(s => s.SnippetCollections)
            .HasForeignKey(sc => sc.SnippetId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship to Collection
        builder.HasOne(sc => sc.Collection)
            .WithMany()
            .HasForeignKey(sc => sc.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index on CollectionId for better query performance when filtering by collection
        builder.HasIndex(sc => sc.CollectionId)
            .HasDatabaseName("IX_SnippetCollections_CollectionId");
    }
}
