using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.DeleteSnippet;

/// <summary>
/// Handles deletion of snippets by processing DeleteSnippetCommand requests.
/// </summary>
public class DeleteSnippetCommandHandler(ISnippetDbContext dbContext) : IRequestHandler<DeleteSnippetCommand, ErrorOr<Unit>>
{

    /// <summary>
    /// Deletes an existing snippet from the database.
    /// </summary>
    /// <param name="request">Command containing snippet ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ErrorOr<Unit>> Handle(DeleteSnippetCommand request, CancellationToken cancellationToken)
    {
        var snippet = await dbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.Id), cancellationToken);

        if (snippet is null)
            return Error.NotFound("Snippet.NotFound", $"Snippet with ID {request.Id} not found");

        dbContext.Snippets.Remove(snippet);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
