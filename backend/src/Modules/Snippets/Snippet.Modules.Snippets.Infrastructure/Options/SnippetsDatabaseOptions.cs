using System.ComponentModel.DataAnnotations;
using Shared.Abstractions.Options;

namespace Snippet.Modules.Snippets.Infrastructure.Options;

/// <summary>
/// Database configuration options for the Snippets module.
/// </summary>
public class SnippetsDatabaseOptions : IOptions
{
    /// <summary>
    /// Configuration section name for binding.
    /// </summary>
    public static string SectionName => "SnippetsDatabase";

    /// <summary>
    /// Gets or sets the PostgreSQL connection string for Snippets database.
    /// </summary>
    [Required(ErrorMessage = "ConnectionString is required")]
    public string ConnectionString { get; set; } = null!;
}
