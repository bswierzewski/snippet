using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snippet.Modules.Snippets.Domain.Enums;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Infrastructure.Persistence.Configuration;

/// <summary>
/// Entity Framework configuration for the Snippet entity.
/// </summary>
public class SnippetConfiguration : IEntityTypeConfiguration<Domain.Aggregates.Snippet>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregates.Snippet> builder)
    {
        builder.ToTable("Snippets");

        // Primary key
        builder.HasKey(s => s.Id);

        // Convert SnippetId value object to Guid
        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => new SnippetId(value))
            .ValueGeneratedNever();

        // UserId
        builder.Property(s => s.UserId)
            .IsRequired();

        // Title
        builder.Property(s => s.Title)
            .IsRequired()
            .HasMaxLength(200);

        // Description
        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        // Content
        builder.Property(s => s.Content)
            .IsRequired()
            .HasMaxLength(50000);

        // Language - convert enum to string
        builder.Property(s => s.Language)
            .IsRequired()
            .HasConversion(
                lang => lang.ToString(),
                value => Enum.Parse<ProgrammingLanguage>(value))
            .HasMaxLength(50);

        // IsFavorite
        builder.Property(s => s.IsFavorite)
            .IsRequired()
            .HasDefaultValue(false);

        // UsageCount
        builder.Property(s => s.UsageCount)
            .IsRequired()
            .HasDefaultValue(0);

        // LastUsedAt
        builder.Property(s => s.LastUsedAt);

        // Audit fields
        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.CreatedBy);

        builder.Property(s => s.ModifiedAt);

        builder.Property(s => s.ModifiedBy);

        // Configure navigation properties to use backing fields
        builder.Navigation(s => s.SnippetTags)
            .HasField("_snippetTags")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(s => s.SnippetCollections)
            .HasField("_snippetCollections")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Indexes for common queries
        builder.HasIndex(s => s.UserId)
            .HasDatabaseName("IX_Snippets_UserId");

        builder.HasIndex(s => s.Language)
            .HasDatabaseName("IX_Snippets_Language");

        builder.HasIndex(s => new { s.UserId, s.IsFavorite })
            .HasDatabaseName("IX_Snippets_UserId_IsFavorite");

        builder.HasIndex(s => new { s.UserId, s.LastUsedAt })
            .HasDatabaseName("IX_Snippets_UserId_LastUsedAt");

        // Ignore domain events collection
        builder.Ignore(s => s.DomainEvents);
    }
}
