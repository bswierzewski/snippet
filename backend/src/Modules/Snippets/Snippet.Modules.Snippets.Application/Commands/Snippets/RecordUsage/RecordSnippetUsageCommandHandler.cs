using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.RecordUsage;

/// <summary>
/// Handles recording snippet usage by processing RecordSnippetUsageCommand requests.
/// </summary>
public class RecordSnippetUsageCommandHandler(ISnippetDbContext dbContext) : IRequestHandler<RecordSnippetUsageCommand, ErrorOr<Unit>>
{

    /// <summary>
    /// Records usage of a snippet by incrementing usage count and updating last used timestamp.
    /// </summary>
    /// <param name="request">Command containing snippet ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ErrorOr<Unit>> Handle(RecordSnippetUsageCommand request, CancellationToken cancellationToken)
    {
        var snippet = await dbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.SnippetId), cancellationToken);

        if (snippet is null)
            return Error.Failure("Error", $"Snippet with ID {request.SnippetId} not found");

        snippet.RecordUsage();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
