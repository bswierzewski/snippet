using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snippet.Modules.Snippets.Domain.Aggregates;
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

        // CollectionIds - store as JSON or create join table
        // Using value conversion to store as JSON array for simplicity
        builder.Property(s => s.CollectionIds)
            .HasConversion(
                ids => System.Text.Json.JsonSerializer.Serialize(ids.Select(id => id.Value).ToList(), (System.Text.Json.JsonSerializerOptions?)null),
                json => System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(json, (System.Text.Json.JsonSerializerOptions?)null)!
                    .Select(guid => new CollectionId(guid))
                    .ToList().AsReadOnly() as IReadOnlyList<CollectionId> ?? new List<CollectionId>().AsReadOnly(),
                new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IReadOnlyList<CollectionId>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList().AsReadOnly()))
            .HasColumnType("jsonb")
            .HasColumnName("CollectionIds");

        // Tags - owned entity collection
        builder.OwnsMany(s => s.Tags, tagsBuilder =>
        {
            tagsBuilder.ToTable("Tags");

            tagsBuilder.WithOwner()
                .HasForeignKey("SnippetId");

            tagsBuilder.HasKey(nameof(Tag.Id), "SnippetId");

            tagsBuilder.Property(t => t.Id)
                .HasConversion(
                    id => id.Value,
                    value => new TagId(value))
                .ValueGeneratedNever();

            tagsBuilder.Property<SnippetId>("SnippetId")
                .HasConversion(
                    id => id.Value,
                    value => new SnippetId(value));

            tagsBuilder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            tagsBuilder.Property(t => t.Color)
                .HasMaxLength(7); // #RRGGBB format

            tagsBuilder.Ignore(t => t.SnippetId);

            // Index on tag name for search
            tagsBuilder.HasIndex(t => t.Name);
        });

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
