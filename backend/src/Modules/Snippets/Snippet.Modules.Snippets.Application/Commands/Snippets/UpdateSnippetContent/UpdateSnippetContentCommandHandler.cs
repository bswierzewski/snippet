using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.UpdateSnippetContent;

/// <summary>
/// Handles updating snippet content by processing UpdateSnippetContentCommand requests.
/// </summary>
public class UpdateSnippetContentCommandHandler : IRequestHandler<UpdateSnippetContentCommand, Result>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;

    public UpdateSnippetContentCommandHandler(ISnippetsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Updates the content of an existing snippet.
    /// </summary>
    /// <param name="request">Command containing snippet ID and new content.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Result> Handle(UpdateSnippetContentCommand request, CancellationToken cancellationToken)
    {
        var snippet = await _writeDbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.Id), cancellationToken);

        if (snippet is null)
            return Result.Failure($"Snippet with ID {request.Id} not found");

        snippet.UpdateContent(request.Content);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
