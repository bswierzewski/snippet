using Shared.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Queries.Snippets.GetSnippetById;

/// <summary>
/// Handles retrieval of a snippet by ID by processing GetSnippetByIdQuery requests.
/// </summary>
public class GetSnippetByIdQueryHandler : IRequestHandler<GetSnippetByIdQuery, Result<GetSnippetByIdDto>>
{
    private readonly ISnippetsReadDbContext _readDbContext;

    public GetSnippetByIdQueryHandler(ISnippetsReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    /// <summary>
    /// Retrieves a snippet by its identifier and maps it to a DTO.
    /// </summary>
    /// <param name="request">Query request containing snippet ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Snippet DTO with full details.</returns>
    public async Task<Result<GetSnippetByIdDto>> Handle(GetSnippetByIdQuery request, CancellationToken cancellationToken)
    {
        var snippet = await _readDbContext.Snippets
            .AsNoTracking()
            .Include(s => s.SnippetTags).ThenInclude(st => st.Tag)
            .Include(s => s.SnippetCollections).ThenInclude(sc => sc.Collection)
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.Id), cancellationToken);

        if (snippet is null)
            return Result<GetSnippetByIdDto>.Failure($"Snippet with ID {request.Id} not found");

        return Result<GetSnippetByIdDto>.Success(new GetSnippetByIdDto(
            snippet.Id.Value,
            snippet.UserId,
            snippet.Title,
            snippet.Description,
            snippet.Content,
            snippet.Language,
            snippet.SnippetCollections.Select(sc => new CollectionSummaryDto(sc.Collection.Id.Value, sc.Collection.Name)).ToList(),
            snippet.SnippetTags.Select(st => new TagDto(st.Tag.Id.Value, st.Tag.Name, st.Tag.Color)).ToList(),
            snippet.IsFavorite,
            snippet.UsageCount,
            snippet.CreatedAt,
            snippet.ModifiedAt,
            snippet.LastUsedAt
        ));
    }
}
