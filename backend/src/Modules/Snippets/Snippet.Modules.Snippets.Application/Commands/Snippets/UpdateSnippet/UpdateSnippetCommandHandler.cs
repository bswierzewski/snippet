using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.UpdateSnippet;

/// <summary>
/// Handles updating snippet by processing UpdateSnippetCommand requests.
/// </summary>
public class UpdateSnippetCommandHandler : IRequestHandler<UpdateSnippetCommand, Result>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;
    private readonly ISnippetsReadDbContext _readDbContext;

    public UpdateSnippetCommandHandler(
        ISnippetsWriteDbContext writeDbContext,
        ISnippetsReadDbContext readDbContext)
    {
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
    }

    /// <summary>
    /// Updates an existing snippet with all provided data.
    /// </summary>
    /// <param name="request">Command containing snippet data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Result> Handle(UpdateSnippetCommand request, CancellationToken cancellationToken)
    {
        var snippet = await _writeDbContext.Snippets
            .Include(s => s.SnippetTags)
            .Include(s => s.SnippetCollections)
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.Id), cancellationToken);

        if (snippet is null)
            return Result.Failure($"Snippet with ID {request.Id} not found");

        // Update basic properties
        snippet.Update(request.Title, request.Description, request.Content, request.Language);

        // Update tags
        var tagIdObjects = request.TagIds.Select(id => new TagId(id)).ToList();
        var tags = await _readDbContext.Tags
            .Where(t => tagIdObjects.Contains(t.Id))
            .ToListAsync(cancellationToken);

        snippet.UpdateTags(tags);

        // Update collections
        var collectionIdObjects = request.CollectionIds.Select(id => new CollectionId(id)).ToList();
        var collections = await _readDbContext.Collections
            .Where(c => collectionIdObjects.Contains(c.Id))
            .ToListAsync(cancellationToken);

        snippet.UpdateCollections(collections);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
