using BuildingBlocks.Abstractions.Abstractions;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;

namespace Snippet.Modules.Snippets.Application.Queries.Tags.GetUserTags;

/// <summary>
/// Handles retrieval of all user tags by processing GetUserTagsQuery requests.
/// </summary>
public class GetUserTagsQueryHandler(ISnippetDbContext dbContext, IUserContext user) : IRequestHandler<GetUserTagsQuery, ErrorOr<IEnumerable<TagDto>>>
{

    /// <summary>
    /// Retrieves all tags for the current user and maps them to DTOs.
    /// Includes snippet count for each tag.
    /// </summary>
    /// <param name="request">Query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of tag DTOs ordered by name.</returns>
    public async Task<ErrorOr<IEnumerable<TagDto>>> Handle(GetUserTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await dbContext.Tags
            .AsNoTracking()
            .Where(t => t.UserId == user.Id)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        var tagIds = tags.Select(t => t.Id).ToList();

        var snippets = await dbContext.Snippets
            .AsNoTracking()
            .Include(s => s.SnippetTags)
            .Where(s => s.SnippetTags.Any(st => tagIds.Contains(st.TagId)))
            .ToListAsync(cancellationToken);

        var snippetCounts = snippets
            .SelectMany(s => s.SnippetTags, (snippet, snippetTag) => snippetTag.TagId)
            .Where(tId => tagIds.Contains(tId))
            .GroupBy(tId => tId)
            .ToDictionary(g => g.Key, g => g.Count());

        return tags.Select(t => new TagDto(
            t.Id.Value,
            t.UserId,
            t.Name,
            t.Color,
            snippetCounts.ContainsKey(t.Id) ? snippetCounts[t.Id] : 0,
            t.CreatedAt
        )).ToList();
    }
}
