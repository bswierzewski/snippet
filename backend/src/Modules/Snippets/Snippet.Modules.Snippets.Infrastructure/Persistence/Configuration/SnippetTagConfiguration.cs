using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snippet.Modules.Snippets.Domain.Entities;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Infrastructure.Persistence.Configuration;

/// <summary>
/// Entity Framework configuration for the SnippetTag join entity.
/// </summary>
public class SnippetTagConfiguration : IEntityTypeConfiguration<SnippetTag>
{
    public void Configure(EntityTypeBuilder<SnippetTag> builder)
    {
        builder.ToTable("SnippetTags");

        // Composite primary key
        builder.HasKey(st => new { st.SnippetId, st.TagId });

        // Convert SnippetId value object to Guid
        builder.Property(st => st.SnippetId)
            .HasConversion(
                id => id.Value,
                value => new SnippetId(value))
            .IsRequired();

        // Convert TagId value object to Guid
        builder.Property(st => st.TagId)
            .HasConversion(
                id => id.Value,
                value => new TagId(value))
            .IsRequired();

        // Configure relationship to Snippet
        builder.HasOne(st => st.Snippet)
            .WithMany(s => s.SnippetTags)
            .HasForeignKey(st => st.SnippetId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship to Tag
        builder.HasOne(st => st.Tag)
            .WithMany()
            .HasForeignKey(st => st.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index on TagId for better query performance when filtering by tag
        builder.HasIndex(st => st.TagId)
            .HasDatabaseName("IX_SnippetTags_TagId");
    }
}
