using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Queries.Snippets.GetCollectionSnippets;

/// <summary>
/// Handles retrieval of snippets in a collection by processing GetCollectionSnippetsQuery requests.
/// </summary>
public class GetCollectionSnippetsQueryHandler(ISnippetDbContext dbContext) : IRequestHandler<GetCollectionSnippetsQuery, ErrorOr<IEnumerable<SnippetSummaryDto>>>
{

    /// <summary>
    /// Retrieves all snippets within a specific collection and maps them to DTOs.
    /// </summary>
    /// <param name="request">Query request containing collection ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of snippet summary DTOs.</returns>
    public async Task<ErrorOr<IEnumerable<SnippetSummaryDto>>> Handle(GetCollectionSnippetsQuery request, CancellationToken cancellationToken)
    {
        var collectionId = new CollectionId(request.CollectionId);

        var collection = await dbContext.Collections
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == collectionId, cancellationToken);

        if (collection is null)
            return Error.NotFound("Collection.NotFound", $"Collection with ID {request.CollectionId} not found");

        var snippets = await dbContext.Snippets
            .AsNoTracking()
            .Include(s => s.SnippetTags).ThenInclude(st => st.Tag)
            .Include(s => s.SnippetCollections).ThenInclude(sc => sc.Collection)
            .Where(s => s.SnippetCollections.Any(sc => sc.CollectionId == collectionId))
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);

        return snippets.Select(s => new SnippetSummaryDto(
            s.Id.Value,
            s.Title,
            s.Description,
            s.Content,
            s.Language,
            s.SnippetCollections.Select(sc => new CollectionSummaryDto(sc.Collection.Id.Value, sc.Collection.Name)).ToList(),
            s.SnippetTags.Select(st => new TagSummaryDto(st.Tag.Id.Value, st.Tag.Name, st.Tag.Color)).ToList(),
            s.IsFavorite,
            s.UsageCount,
            s.CreatedAt,
            s.LastUsedAt
        )).ToList();
    }
}
