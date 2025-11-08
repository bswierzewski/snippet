using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;

namespace Snippet.Modules.Snippets.Application.Queries.Tags.GetTags;

/// <summary>
/// Fast handler for tag search optimized for autocomplete/search scenarios.
/// Returns minimal data without joins for maximum performance.
/// </summary>
public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, Result<IEnumerable<TagSearchDto>>>
{
    private readonly ISnippetsReadDbContext _readDbContext;
    private readonly IUser _user;

    public GetTagsQueryHandler(ISnippetsReadDbContext readDbContext, IUser user)
    {
        _readDbContext = readDbContext;
        _user = user;
    }

    /// <summary>
    /// Searches tags by name with optimal performance.
    /// Returns up to 20 results ordered by name.
    /// </summary>
    /// <param name="request">Query with optional search term.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Lightweight collection of tag search results.</returns>
    public async Task<Result<IEnumerable<TagSearchDto>>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var query = _readDbContext.Tags
            .AsNoTracking()
            .Where(t => t.UserId == _user.Id);

        // Filter by search term if provided
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLowerInvariant();
            query = query.Where(t => t.Name.Contains(searchTerm));
        }

        var tags = await query
            .OrderBy(t => t.Name)
            .Take(20)
            .Select(t => new TagSearchDto(
                t.Id.Value,
                t.Name
            ))
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<TagSearchDto>>.Success(tags);
    }
}
