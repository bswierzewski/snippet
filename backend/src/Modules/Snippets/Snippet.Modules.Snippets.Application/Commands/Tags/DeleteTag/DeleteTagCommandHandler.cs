using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Tags.DeleteTag;

/// <summary>
/// Handles deletion of tags by processing DeleteTagCommand requests.
/// </summary>
public class DeleteTagCommandHandler(ISnippetDbContext dbContext) : IRequestHandler<DeleteTagCommand, ErrorOr<Unit>>
{

    /// <summary>
    /// Deletes an existing tag from the database.
    /// </summary>
    /// <param name="request">Command containing tag ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ErrorOr<Unit>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = await dbContext.Tags
            .FirstOrDefaultAsync(t => t.Id == new TagId(request.Id), cancellationToken);

        if (tag is null)
            return Error.NotFound("Tag.NotFound", $"Tag with ID {request.Id} not found");

        dbContext.Tags.Remove(tag);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
