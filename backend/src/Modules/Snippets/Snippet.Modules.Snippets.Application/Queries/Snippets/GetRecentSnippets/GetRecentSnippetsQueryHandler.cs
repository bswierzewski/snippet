using BuildingBlocks.Abstractions.Abstractions;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;

namespace Snippet.Modules.Snippets.Application.Queries.Snippets.GetRecentSnippets;

/// <summary>
/// Handles retrieval of recently used snippets by processing GetRecentSnippetsQuery requests.
/// </summary>
public class GetRecentSnippetsQueryHandler(ISnippetDbContext dbContext, IUserContext user) : IRequestHandler<GetRecentSnippetsQuery, ErrorOr<IEnumerable<SnippetSummaryDto>>>
{

    /// <summary>
    /// Retrieves recently used snippets for the current user and maps them to DTOs.
    /// </summary>
    /// <param name="request">Query request with limit.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of recently used snippet summary DTOs.</returns>
    public async Task<ErrorOr<IEnumerable<SnippetSummaryDto>>> Handle(GetRecentSnippetsQuery request, CancellationToken cancellationToken)
    {
        var snippets = await dbContext.Snippets
            .AsNoTracking()
            .Include(s => s.SnippetTags).ThenInclude(st => st.Tag)
            .Include(s => s.SnippetCollections).ThenInclude(sc => sc.Collection)
            .Where(s => s.UserId == user.Id && s.LastUsedAt != null)
            .OrderByDescending(s => s.LastUsedAt)
            .Take(request.Limit)
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
