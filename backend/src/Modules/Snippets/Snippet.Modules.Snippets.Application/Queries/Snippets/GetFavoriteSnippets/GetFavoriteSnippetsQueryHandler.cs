using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;

namespace Snippet.Modules.Snippets.Application.Queries.Snippets.GetFavoriteSnippets;

/// <summary>
/// Handles retrieval of favorite snippets by processing GetFavoriteSnippetsQuery requests.
/// </summary>
public class GetFavoriteSnippetsQueryHandler : IRequestHandler<GetFavoriteSnippetsQuery, Result<IEnumerable<SnippetSummaryDto>>>
{
    private readonly ISnippetsReadDbContext _readDbContext;
    private readonly IUser _user;

    public GetFavoriteSnippetsQueryHandler(ISnippetsReadDbContext readDbContext, IUser user)
    {
        _readDbContext = readDbContext;
        _user = user;
    }

    /// <summary>
    /// Retrieves all favorite snippets for the current user and maps them to DTOs.
    /// </summary>
    /// <param name="request">Query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of favorite snippet summary DTOs.</returns>
    public async Task<Result<IEnumerable<SnippetSummaryDto>>> Handle(GetFavoriteSnippetsQuery request, CancellationToken cancellationToken)
    {
        var snippets = await _readDbContext.Snippets
            .AsNoTracking()
            .Include(s => s.SnippetTags).ThenInclude(st => st.Tag)
            .Include(s => s.SnippetCollections).ThenInclude(sc => sc.Collection)
            .Where(s => s.UserId == _user.Id && s.IsFavorite)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<SnippetSummaryDto>>.Success(snippets.Select(s => new SnippetSummaryDto(
            s.Id.Value,
            s.Title,
            s.Language,
            s.SnippetCollections.Select(sc => new CollectionSummaryDto(sc.Collection.Id.Value, sc.Collection.Name)).ToList(),
            s.SnippetTags.Select(st => new TagSummaryDto(st.Tag.Id.Value, st.Tag.Name, st.Tag.Color)).ToList(),
            s.IsFavorite,
            s.UsageCount,
            s.CreatedAt,
            s.LastUsedAt
        )));
    }
}
