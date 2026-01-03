using BuildingBlocks.Abstractions.Abstractions;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Application.Queries.Collections.GetCollectionById;

namespace Snippet.Modules.Snippets.Application.Queries.Collections.GetUserCollections;

/// <summary>
/// Handles retrieval of all user collections by processing GetUserCollectionsQuery requests.
/// </summary>
public class GetUserCollectionsQueryHandler(ISnippetDbContext dbContext, IUserContext user) : IRequestHandler<GetUserCollectionsQuery, ErrorOr<IEnumerable<CollectionDto>>>
{

    /// <summary>
    /// Retrieves all collections for the current user and maps them to DTOs.
    /// </summary>
    /// <param name="request">Query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of collection DTOs.</returns>
    public async Task<ErrorOr<IEnumerable<CollectionDto>>> Handle(GetUserCollectionsQuery request, CancellationToken cancellationToken)
    {
        var collections = await dbContext.Collections
            .AsNoTracking()
            .Where(c => c.UserId == user.Id)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);

        var collectionIds = collections.Select(c => c.Id).ToList();

        var snippets = await dbContext.Snippets
            .AsNoTracking()
            .Include(s => s.SnippetCollections)
            .Where(s => s.SnippetCollections.Any(sc => collectionIds.Contains(sc.CollectionId)))
            .ToListAsync(cancellationToken);

        var snippetCounts = snippets
            .SelectMany(s => s.SnippetCollections, (snippet, snippetCollection) => snippetCollection.CollectionId)
            .Where(cId => collectionIds.Contains(cId))
            .GroupBy(cId => cId)
            .ToDictionary(g => g.Key, g => g.Count());

        return collections.Select(c => new CollectionDto(
            c.Id.Value,
            c.UserId,
            c.Name,
            c.Description,
            c.Color,
            c.Icon,
            c.SortOrder,
            snippetCounts.ContainsKey(c.Id) ? snippetCounts[c.Id] : 0,
            c.CreatedAt
        )).ToList();
    }
}
