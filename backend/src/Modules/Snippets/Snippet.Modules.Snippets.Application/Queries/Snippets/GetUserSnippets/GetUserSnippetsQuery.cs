using BuildingBlocks.Application.Models;
using MediatR;
using Snippet.Modules.Snippets.Domain.Enums;

namespace Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;

/// <summary>
/// Query to retrieve all snippets owned by the current user.
/// </summary>
public record GetUserSnippetsQuery() : IRequest<Result<IEnumerable<SnippetSummaryDto>>>;

/// <summary>
/// Data transfer object containing summary snippet information for display in lists.
/// </summary>
/// <param name="Id">Snippet unique identifier.</param>
/// <param name="Title">Snippet title.</param>
/// <param name="Language">Programming language for syntax highlighting.</param>
/// <param name="Collections">List of collections this snippet belongs to.</param>
/// <param name="Tags">List of tags associated with the snippet.</param>
/// <param name="IsFavorite">Whether the snippet is marked as favorite.</param>
/// <param name="UsageCount">Number of times the snippet has been used.</param>
/// <param name="CreatedAt">Date and time when the snippet was created.</param>
/// <param name="LastUsedAt">Date and time when the snippet was last used.</param>
public record SnippetSummaryDto(
    Guid Id,
    string Title,
    ProgrammingLanguage Language,
    List<CollectionSummaryDto> Collections,
    List<TagSummaryDto> Tags,
    bool IsFavorite,
    int UsageCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastUsedAt
);

/// <summary>
/// Data transfer object for collection summary information.
/// </summary>
/// <param name="Id">Collection unique identifier.</param>
/// <param name="Name">Collection name.</param>
public record CollectionSummaryDto(
    Guid Id,
    string Name
);

/// <summary>
/// Data transfer object for tag summary information.
/// </summary>
/// <param name="Id">Tag unique identifier.</param>
/// <param name="Name">Tag name.</param>
/// <param name="Color">Optional tag color in hexadecimal format.</param>
public record TagSummaryDto(
    Guid Id,
    string Name,
    string? Color
);
