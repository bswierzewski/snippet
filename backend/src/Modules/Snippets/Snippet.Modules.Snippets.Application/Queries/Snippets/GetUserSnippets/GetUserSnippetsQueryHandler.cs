using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;

namespace Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;

/// <summary>
/// Handles retrieval of all user snippets by processing GetUserSnippetsQuery requests.
/// </summary>
public class GetUserSnippetsQueryHandler : IRequestHandler<GetUserSnippetsQuery, Result<IEnumerable<SnippetSummaryDto>>>
{
    private readonly ISnippetsReadDbContext _readDbContext;
    private readonly IUser _user;

    public GetUserSnippetsQueryHandler(ISnippetsReadDbContext readDbContext, IUser user)
    {
        _readDbContext = readDbContext;
        _user = user;
    }

    /// <summary>
    /// Retrieves all snippets for the current user and maps them to DTOs.
    /// </summary>
    /// <param name="request">Query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of snippet summary DTOs.</returns>
    public async Task<Result<IEnumerable<SnippetSummaryDto>>> Handle(GetUserSnippetsQuery request, CancellationToken cancellationToken)
    {
        var snippets = await _readDbContext.Snippets
            .AsNoTracking()
            .Where(s => s.UserId == _user.Id)
            .OrderByDescending(s => s.CreatedAt)
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
