using Shared.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.RecordUsage;

/// <summary>
/// Handles recording snippet usage by processing RecordSnippetUsageCommand requests.
/// </summary>
public class RecordSnippetUsageCommandHandler : IRequestHandler<RecordSnippetUsageCommand, Result>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;

    public RecordSnippetUsageCommandHandler(ISnippetsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Records usage of a snippet by incrementing usage count and updating last used timestamp.
    /// </summary>
    /// <param name="request">Command containing snippet ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Result> Handle(RecordSnippetUsageCommand request, CancellationToken cancellationToken)
    {
        var snippet = await _writeDbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.SnippetId), cancellationToken);

        if (snippet is null)
            return Result.Failure($"Snippet with ID {request.SnippetId} not found");

        snippet.RecordUsage();

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
