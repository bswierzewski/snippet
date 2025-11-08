using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Tags.DeleteTag;

/// <summary>
/// Handles deletion of tags by processing DeleteTagCommand requests.
/// </summary>
public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, Result>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;

    public DeleteTagCommandHandler(ISnippetsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Deletes an existing tag from the database.
    /// </summary>
    /// <param name="request">Command containing tag ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Result> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await _writeDbContext.Tags
            .FirstOrDefaultAsync(t => t.Id == new TagId(request.Id), cancellationToken);

        if (tag is null)
            return Result.Failure($"Tag with ID {request.Id} not found");

        _writeDbContext.Tags.Remove(tag);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
