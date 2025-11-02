using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.DeleteSnippet;

/// <summary>
/// Handles deletion of snippets by processing DeleteSnippetCommand requests.
/// </summary>
public class DeleteSnippetCommandHandler : IRequestHandler<DeleteSnippetCommand, Result>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;

    public DeleteSnippetCommandHandler(ISnippetsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Deletes an existing snippet from the database.
    /// </summary>
    /// <param name="request">Command containing snippet ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Result> Handle(DeleteSnippetCommand request, CancellationToken cancellationToken)
    {
        var snippet = await _writeDbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.Id), cancellationToken);

        if (snippet is null)
            return Result.Failure($"Snippet with ID {request.Id} not found");

        _writeDbContext.Snippets.Remove(snippet);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
