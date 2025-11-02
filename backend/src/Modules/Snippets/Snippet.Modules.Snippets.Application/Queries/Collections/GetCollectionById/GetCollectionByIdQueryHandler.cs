using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Queries.Collections.GetCollectionById;

/// <summary>
/// Handles retrieval of a collection by ID by processing GetCollectionByIdQuery requests.
/// </summary>
public class GetCollectionByIdQueryHandler : IRequestHandler<GetCollectionByIdQuery, Result<CollectionDto>>
{
    private readonly ISnippetsReadDbContext _readDbContext;

    public GetCollectionByIdQueryHandler(ISnippetsReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    /// <summary>
    /// Retrieves a collection by its identifier and maps it to a DTO.
    /// </summary>
    /// <param name="request">Query request containing collection ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection DTO with full details.</returns>
    public async Task<Result<CollectionDto>> Handle(GetCollectionByIdQuery request, CancellationToken cancellationToken)
    {
        var collection = await _readDbContext.Collections
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == new CollectionId(request.Id), cancellationToken);

        if (collection is null)
            return Result<CollectionDto>.Failure($"Collection with ID {request.Id} not found");

        var snippetCount = await _readDbContext.Snippets
            .AsNoTracking()
            .CountAsync(s => s.SnippetCollections.Any(sc => sc.CollectionId == collection.Id), cancellationToken);

        return Result<CollectionDto>.Success(new CollectionDto(
            collection.Id.Value,
            collection.UserId,
            collection.Name,
            collection.Description,
            collection.Color,
            collection.Icon,
            collection.SortOrder,
            snippetCount,
            collection.CreatedAt
        ));
    }
}
