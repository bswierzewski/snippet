using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Queries.Collections.GetCollectionById;

/// <summary>
/// Handles retrieval of a collection by ID by processing GetCollectionByIdQuery requests.
/// </summary>
public class GetCollectionByIdQueryHandler(ISnippetDbContext dbContext) : IRequestHandler<GetCollectionByIdQuery, ErrorOr<CollectionDto>>
{

    /// <summary>
    /// Retrieves a collection by its identifier and maps it to a DTO.
    /// </summary>
    /// <param name="request">Query request containing collection ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection DTO with full details.</returns>
    public async Task<ErrorOr<CollectionDto>> Handle(GetCollectionByIdQuery request, CancellationToken cancellationToken)
    {
        var collection = await dbContext.Collections
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == new CollectionId(request.Id), cancellationToken);

        if (collection is null)
            return Error.NotFound("Collection.NotFound", $"Collection with ID {request.Id} not found");

        var snippetCount = await dbContext.Snippets
            .AsNoTracking()
            .CountAsync(s => s.SnippetCollections.Any(sc => sc.CollectionId == collection.Id), cancellationToken);

        return new CollectionDto(
            collection.Id.Value,
            collection.UserId,
            collection.Name,
            collection.Description,
            collection.Color,
            collection.Icon,
            collection.SortOrder,
            snippetCount,
            collection.CreatedAt
        );
    }
}
