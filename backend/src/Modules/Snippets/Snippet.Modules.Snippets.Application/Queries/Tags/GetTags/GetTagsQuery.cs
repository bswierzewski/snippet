using ErrorOr;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Queries.Tags.GetTags;

/// <summary>
/// Fast query to search tags by name for autocomplete/search functionality.
/// </summary>
/// <param name="SearchTerm">Optional search term to filter tags by name (case-insensitive).</param>
public record GetTagsQuery(string? SearchTerm = null) : IRequest<ErrorOr<IEnumerable<TagSearchDto>>>;

/// <summary>
/// Lightweight DTO for tag search results containing only essential information.
/// </summary>
/// <param name="Id">Tag unique identifier.</param>
/// <param name="Name">Tag name (always lowercase).</param>
public record TagSearchDto(
    Guid Id,
    string Name
);
