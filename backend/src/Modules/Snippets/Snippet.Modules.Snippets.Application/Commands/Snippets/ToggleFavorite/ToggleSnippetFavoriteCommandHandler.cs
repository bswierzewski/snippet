using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.ToggleFavorite;

/// <summary>
/// Handles toggling snippet favorite status by processing ToggleSnippetFavoriteCommand requests.
/// </summary>
public class ToggleSnippetFavoriteCommandHandler(ISnippetDbContext dbContext) : IRequestHandler<ToggleSnippetFavoriteCommand, ErrorOr<Unit>>
{

    /// <summary>
    /// Toggles the favorite status of a snippet.
    /// </summary>
    /// <param name="request">Command containing snippet ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ErrorOr<Unit>> Handle(ToggleSnippetFavoriteCommand request, CancellationToken cancellationToken)
    {
        var snippet = await dbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.SnippetId), cancellationToken);

        if (snippet is null)
            return Error.Failure("Error", $"Snippet with ID {request.SnippetId} not found");

        snippet.ToggleFavorite();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
