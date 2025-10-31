using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Queries.Snippets.SearchSnippets;

/// <summary>
/// Handles snippet search by processing SearchSnippetsQuery requests.
/// </summary>
public class SearchSnippetsQueryHandler : IRequestHandler<SearchSnippetsQuery, Result<SearchSnippetsResponse>>
{
    private readonly ISnippetsReadDbContext _readDbContext;
    private readonly IUser _user;

    public SearchSnippetsQueryHandler(ISnippetsReadDbContext readDbContext, IUser user)
    {
        _readDbContext = readDbContext;
        _user = user;
    }

    /// <summary>
    /// Searches for snippets based on provided criteria and returns paginated results.
    /// </summary>
    /// <param name="request">Query request with search criteria.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated search results.</returns>
    public async Task<Result<SearchSnippetsResponse>> Handle(SearchSnippetsQuery request, CancellationToken cancellationToken)
    {
        var query = _readDbContext.Snippets
            .AsNoTracking()
            .Where(s => s.UserId == _user.Id);

        // Apply search term filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(s =>
                s.Title.ToLower().Contains(searchTerm) ||
                s.Content.ToLower().Contains(searchTerm));
        }

        // Apply language filter
        if (request.Languages != null && request.Languages.Any())
        {
            query = query.Where(s => request.Languages.Contains(s.Language));
        }

        // Apply favorites filter
        if (request.FavoritesOnly.HasValue && request.FavoritesOnly.Value)
        {
            query = query.Where(s => s.IsFavorite);
        }

        // Apply collection filter
        if (request.CollectionId.HasValue)
        {
            var collectionId = new CollectionId(request.CollectionId.Value);
            query = query.Where(s => s.CollectionIds.Contains(collectionId));
        }

        // Apply tags filter (OR logic)
        if (request.Tags != null && request.Tags.Any())
        {
            var tagNames = request.Tags.Select(t => t.ToLower()).ToList();
            query = query.Where(s => s.Tags.Any(t => tagNames.Contains(t.Name.ToLower())));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var snippets = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Fetch collection names
        var allCollectionIds = snippets
            .SelectMany(s => s.CollectionIds)
            .Distinct()
            .ToList();

        var collections = await _readDbContext.Collections
            .AsNoTracking()
            .Where(c => allCollectionIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        var snippetDtos = snippets.Select(s => new SnippetSummaryDto(
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
        )).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return Result<SearchSnippetsResponse>.Success(new SearchSnippetsResponse(
            snippetDtos,
            totalCount,
            request.PageNumber,
            request.PageSize,
            totalPages
        ));
    }
}
