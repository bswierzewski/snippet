using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Snippet.Modules.Snippets.Domain.Aggregates;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Infrastructure.Persistence.Configuration;

/// <summary>
/// Entity Framework configuration for the Collection entity.
/// </summary>
public class CollectionConfiguration : IEntityTypeConfiguration<Collection>
{
    public void Configure(EntityTypeBuilder<Collection> builder)
    {
        builder.ToTable("Collections");

        // Primary key
        builder.HasKey(c => c.Id);

        // Convert CollectionId value object to Guid
        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => new CollectionId(value))
            .ValueGeneratedNever();

        // UserId
        builder.Property(c => c.UserId)
            .IsRequired();

        // Name
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Description
        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        // Color
        builder.Property(c => c.Color)
            .HasMaxLength(7); // #RRGGBB format

        // Icon
        builder.Property(c => c.Icon)
            .HasMaxLength(50);

        // SortOrder
        builder.Property(c => c.SortOrder)
            .IsRequired()
            .HasDefaultValue(0);

        // Audit fields
        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.CreatedBy);

        builder.Property(c => c.ModifiedAt);

        builder.Property(c => c.ModifiedBy);

        // Indexes for common queries
        builder.HasIndex(c => c.UserId)
            .HasDatabaseName("IX_Collections_UserId");

        builder.HasIndex(c => new { c.UserId, c.SortOrder })
            .HasDatabaseName("IX_Collections_UserId_SortOrder");

        // Ignore domain events collection
        builder.Ignore(c => c.DomainEvents);
    }
}
