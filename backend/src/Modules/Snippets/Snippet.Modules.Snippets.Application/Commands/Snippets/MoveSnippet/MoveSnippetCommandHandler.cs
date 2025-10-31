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

    public MoveSnippetCommandHandler(ISnippetsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Updates the collections a snippet belongs to.
    /// </summary>
    /// <param name="request">Command containing snippet ID and target collection IDs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Result> Handle(MoveSnippetCommand request, CancellationToken cancellationToken)
    {
        var snippet = await _writeDbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.SnippetId), cancellationToken);

        if (snippet is null)
            return Result.Failure($"Snippet with ID {request.SnippetId} not found");

        var collectionIds = request.CollectionIds
            .Select(id => new CollectionId(id))
            .ToList();

        snippet.UpdateCollections(collectionIds);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
