using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.RemoveTag;

/// <summary>
/// Handles removing tags from snippets by processing RemoveTagCommand requests.
/// </summary>
public class RemoveTagCommandHandler : IRequestHandler<RemoveTagCommand, Result>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;

    public RemoveTagCommandHandler(ISnippetsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Removes a tag from an existing snippet.
    /// </summary>
    /// <param name="request">Command containing snippet ID and tag ID to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Result> Handle(RemoveTagCommand request, CancellationToken cancellationToken)
    {
        var snippet = await _writeDbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.SnippetId), cancellationToken);

        if (snippet is null)
            return Result.Failure($"Snippet with ID {request.SnippetId} not found");

        snippet.RemoveTag(new TagId(request.TagId));

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
