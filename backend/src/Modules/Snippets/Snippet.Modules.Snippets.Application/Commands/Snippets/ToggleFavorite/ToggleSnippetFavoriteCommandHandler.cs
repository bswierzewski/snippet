using Shared.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.ToggleFavorite;

/// <summary>
/// Handles toggling snippet favorite status by processing ToggleSnippetFavoriteCommand requests.
/// </summary>
public class ToggleSnippetFavoriteCommandHandler : IRequestHandler<ToggleSnippetFavoriteCommand, Result>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;

    public ToggleSnippetFavoriteCommandHandler(ISnippetsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Toggles the favorite status of a snippet.
    /// </summary>
    /// <param name="request">Command containing snippet ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Result> Handle(ToggleSnippetFavoriteCommand request, CancellationToken cancellationToken)
    {
        var snippet = await _writeDbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.SnippetId), cancellationToken);

        if (snippet is null)
            return Result.Failure($"Snippet with ID {request.SnippetId} not found");

        snippet.ToggleFavorite();

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
