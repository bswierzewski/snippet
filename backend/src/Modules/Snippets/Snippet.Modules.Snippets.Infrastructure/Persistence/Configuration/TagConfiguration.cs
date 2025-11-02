using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snippet.Modules.Snippets.Domain.Aggregates;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Infrastructure.Persistence.Configuration;

/// <summary>
/// Entity Framework configuration for the Tag aggregate root.
/// </summary>
public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        // Primary key
        builder.HasKey(t => t.Id);

        // Convert TagId value object to Guid
        builder.Property(t => t.Id)
            .HasConversion(
                id => id.Value,
                value => new TagId(value))
            .ValueGeneratedNever();

        // UserId
        builder.Property(t => t.UserId)
            .IsRequired();

        // Name
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Color
        builder.Property(t => t.Color)
            .HasMaxLength(50);

        // Audit fields
        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.CreatedBy);

        builder.Property(t => t.ModifiedAt);

        builder.Property(t => t.ModifiedBy);

        // Indexes
        builder.HasIndex(t => t.UserId)
            .HasDatabaseName("IX_Tags_UserId");

        builder.HasIndex(t => new { t.UserId, t.Name })
            .HasDatabaseName("IX_Tags_UserId_Name")
            .IsUnique();

        // Ignore domain events collection
        builder.Ignore(t => t.DomainEvents);
    }
}
