using BuildingBlocks.Application.Models;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Queries.Collections.GetCollectionById;

/// <summary>
/// Query to retrieve a specific collection by its unique identifier.
/// </summary>
/// <param name="Id">Collection unique identifier.</param>
public record GetCollectionByIdQuery(Guid Id) : IRequest<Result<CollectionDto>>;

/// <summary>
/// Data transfer object containing full collection information.
/// </summary>
/// <param name="Id">Collection unique identifier.</param>
/// <param name="UserId">User identifier who owns the collection.</param>
/// <param name="Name">Collection name.</param>
/// <param name="Description">Optional collection description.</param>
/// <param name="Color">Optional color in hexadecimal format.</param>
/// <param name="Icon">Optional icon name or emoji.</param>
/// <param name="SortOrder">Sort order for display.</param>
/// <param name="SnippetCount">Number of snippets in this collection.</param>
/// <param name="CreatedAt">Date and time when the collection was created.</param>
public record CollectionDto(
    Guid Id,
    Guid UserId,
    string Name,
    string? Description,
    string? Color,
    string? Icon,
    int SortOrder,
    int SnippetCount,
    DateTimeOffset CreatedAt
);
