using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Collections.DeleteCollection;

/// <summary>
/// Handles deletion of collections by processing DeleteCollectionCommand requests.
/// </summary>
public class DeleteCollectionCommandHandler(ISnippetDbContext dbContext) : IRequestHandler<DeleteCollectionCommand, ErrorOr<Unit>>
{
    /// <summary>
    /// Deletes an existing collection from the database.
    /// </summary>
    /// <param name="request">Command containing collection ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ErrorOr<Unit>> Handle(DeleteCollectionCommand request, CancellationToken cancellationToken)
    {
        var collection = await dbContext.Collections
            .FirstOrDefaultAsync(c => c.Id == new CollectionId(request.Id), cancellationToken);

        if (collection is null)
            return Error.NotFound("Collection.NotFound", $"Collection with ID {request.Id} not found");

        dbContext.Collections.Remove(collection);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
