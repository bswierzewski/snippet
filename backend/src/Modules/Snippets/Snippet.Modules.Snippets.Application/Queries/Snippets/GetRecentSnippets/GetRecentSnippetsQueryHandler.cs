using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;

namespace Snippet.Modules.Snippets.Application.Queries.Snippets.GetRecentSnippets;

/// <summary>
/// Handles retrieval of recently used snippets by processing GetRecentSnippetsQuery requests.
/// </summary>
public class GetRecentSnippetsQueryHandler : IRequestHandler<GetRecentSnippetsQuery, Result<IEnumerable<SnippetSummaryDto>>>
{
    private readonly ISnippetsReadDbContext _readDbContext;
    private readonly IUser _user;

    public GetRecentSnippetsQueryHandler(ISnippetsReadDbContext readDbContext, IUser user)
    {
        _readDbContext = readDbContext;
        _user = user;
    }

    /// <summary>
    /// Retrieves recently used snippets for the current user and maps them to DTOs.
    /// </summary>
    /// <param name="request">Query request with limit.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of recently used snippet summary DTOs.</returns>
    public async Task<Result<IEnumerable<SnippetSummaryDto>>> Handle(GetRecentSnippetsQuery request, CancellationToken cancellationToken)
    {
        var snippets = await _readDbContext.Snippets
            .AsNoTracking()
            .Where(s => s.UserId == _user.Id && s.LastUsedAt != null)
            .OrderByDescending(s => s.LastUsedAt)
            .Take(request.Limit)
            .ToListAsync(cancellationToken);

        var allCollectionIds = snippets
            .SelectMany(s => s.CollectionIds)
            .Distinct()
            .ToList();

        var collections = await _readDbContext.Collections
            .AsNoTracking()
            .Where(c => allCollectionIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        return Result<IEnumerable<SnippetSummaryDto>>.Success(snippets.Select(s => new SnippetSummaryDto(
            s.Id.Value,
            s.Title,
            s.Language,
            s.CollectionIds
                .Where(cId => collections.ContainsKey(cId))
                .Select(cId => new CollectionSummaryDto(cId.Value, collections[cId]))
                .ToList(),
            s.Tags.Select(t => new TagSummaryDto(t.Id.Value, t.Name, t.Color)).ToList(),
            s.IsFavorite,
            s.UsageCount,
            s.CreatedAt,
            s.LastUsedAt
        )));
    }
}
