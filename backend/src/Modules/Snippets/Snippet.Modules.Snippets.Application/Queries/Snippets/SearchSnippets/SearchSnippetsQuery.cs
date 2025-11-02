using BuildingBlocks.Application.Models;
using MediatR;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;
using Snippet.Modules.Snippets.Domain.Enums;

namespace Snippet.Modules.Snippets.Application.Queries.Snippets.SearchSnippets;

/// <summary>
/// Query to search snippets with multiple filter criteria.
/// </summary>
/// <param name="SearchTerm">Optional search term to match against title and content.</param>
/// <param name="Tags">Optional list of tag names to filter by (OR logic).</param>
/// <param name="Languages">Optional list of programming languages to filter by (OR logic).</param>
/// <param name="FavoritesOnly">Optional filter to show only favorite snippets.</param>
/// <param name="CollectionId">Optional collection identifier to filter by.</param>
/// <param name="PageNumber">Page number for pagination (default 1).</param>
/// <param name="PageSize">Page size for pagination (default 50).</param>
public record SearchSnippetsQuery(
    string? SearchTerm,
    List<string>? Tags,
    List<ProgrammingLanguage>? Languages,
    bool? FavoritesOnly,
    Guid? CollectionId,
    int PageNumber = 1,
    int PageSize = 50
) : IRequest<Result<SearchSnippetsResponse>>;

/// <summary>
/// Response containing search results with pagination information.
/// </summary>
/// <param name="Snippets">List of snippets matching the search criteria.</param>
/// <param name="TotalCount">Total number of snippets matching the criteria.</param>
/// <param name="PageNumber">Current page number.</param>
/// <param name="PageSize">Current page size.</param>
/// <param name="TotalPages">Total number of pages.</param>
public record SearchSnippetsResponse(
    List<SnippetSummaryDto> Snippets,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);
