using BuildingBlocks.Application.Models;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Queries.Tags.GetUserTags;

/// <summary>
/// Query to retrieve all tags owned by the current user.
/// </summary>
public record GetUserTagsQuery() : IRequest<Result<IEnumerable<TagDto>>>;

/// <summary>
/// Data transfer object containing tag information.
/// </summary>
/// <param name="Id">Tag unique identifier.</param>
/// <param name="UserId">User identifier who owns the tag.</param>
/// <param name="Name">Tag name (always lowercase).</param>
/// <param name="Color">Optional color in hexadecimal format.</param>
/// <param name="SnippetCount">Number of snippets using this tag.</param>
/// <param name="CreatedAt">Date and time when the tag was created.</param>
public record TagDto(
    Guid Id,
    Guid UserId,
    string Name,
    string? Color,
    int SnippetCount,
    DateTimeOffset CreatedAt
);
