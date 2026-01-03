using BuildingBlocks.Abstractions.Abstractions;
using Snippet.Modules.Snippets.Domain;
using System.ComponentModel.DataAnnotations;

namespace Snippet.Modules.Snippets.Infrastructure.Options;

public class SnippetsDatabaseOptions : IOptions
{
    public static string SectionName => $"Modules:{Module.Name}";

    [Required(ErrorMessage = "ConnectionString is required")]
    public string ConnectionString { get; set; } = null!;
}
