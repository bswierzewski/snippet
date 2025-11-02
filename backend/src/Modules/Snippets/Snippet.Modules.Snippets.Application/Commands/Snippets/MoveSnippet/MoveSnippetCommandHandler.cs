using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.MoveSnippet;

/// <summary>
/// Handles updating snippet collection assignments by processing MoveSnippetCommand requests.
/// </summary>
public class MoveSnippetCommandHandler : IRequestHandler<MoveSnippetCommand, Result>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;
    private readonly ISnippetsReadDbContext _readDbContext;

    public MoveSnippetCommandHandler(
        ISnippetsWriteDbContext writeDbContext,
        ISnippetsReadDbContext readDbContext)
    {
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
    }

    /// <summary>
    /// Updates the collections a snippet belongs to.
    /// </summary>
    /// <param name="request">Command containing snippet ID and target collection IDs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Result> Handle(MoveSnippetCommand request, CancellationToken cancellationToken)
    {
        var snippet = await _writeDbContext.Snippets
            .Include(s => s.SnippetCollections)
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.SnippetId), cancellationToken);

        if (snippet is null)
            return Result.Failure($"Snippet with ID {request.SnippetId} not found");

        // Fetch collections from database
        var collectionIdObjects = request.CollectionIds.Select(id => new CollectionId(id)).ToList();
        var collections = await _readDbContext.Collections
            .Where(c => collectionIdObjects.Contains(c.Id))
            .ToListAsync(cancellationToken);

        snippet.UpdateCollections(collections);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
