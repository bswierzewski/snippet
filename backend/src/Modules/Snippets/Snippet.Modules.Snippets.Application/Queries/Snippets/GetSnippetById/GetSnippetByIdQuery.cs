using BuildingBlocks.Application.Models;
using MediatR;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;
using Snippet.Modules.Snippets.Domain.Enums;

namespace Snippet.Modules.Snippets.Application.Queries.Snippets.GetSnippetById;

/// <summary>
/// Query to retrieve a specific snippet by its unique identifier.
/// </summary>
/// <param name="Id">Snippet unique identifier.</param>
public record GetSnippetByIdQuery(Guid Id) : IRequest<Result<GetSnippetByIdDto>>;

/// <summary>
/// Data transfer object containing full snippet information.
/// </summary>
/// <param name="Id">Snippet unique identifier.</param>
/// <param name="UserId">User identifier who owns the snippet.</param>
/// <param name="Title">Snippet title.</param>
/// <param name="Description">Optional snippet description.</param>
/// <param name="Content">Snippet content (code, query, or prompt text).</param>
/// <param name="Language">Programming language for syntax highlighting.</param>
/// <param name="Collections">List of collections this snippet belongs to.</param>
/// <param name="Tags">List of tags associated with the snippet.</param>
/// <param name="IsFavorite">Whether the snippet is marked as favorite.</param>
/// <param name="UsageCount">Number of times the snippet has been used.</param>
/// <param name="CreatedAt">Date and time when the snippet was created.</param>
/// <param name="ModifiedAt">Date and time when the snippet was last modified.</param>
/// <param name="LastUsedAt">Date and time when the snippet was last used.</param>
public record GetSnippetByIdDto(
    Guid Id,
    Guid UserId,
    string Title,
    string? Description,
    string Content,
    ProgrammingLanguage Language,
    List<CollectionSummaryDto> Collections,
    List<TagDto> Tags,
    bool IsFavorite,
    int UsageCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ModifiedAt,
    DateTimeOffset? LastUsedAt
);

/// <summary>
/// Data transfer object for tag information.
/// </summary>
/// <param name="Id">Tag unique identifier.</param>
/// <param name="Name">Tag name.</param>
/// <param name="Color">Optional tag color in hexadecimal format.</param>
public record TagDto(
    Guid Id,
    string Name,
    string? Color
);
