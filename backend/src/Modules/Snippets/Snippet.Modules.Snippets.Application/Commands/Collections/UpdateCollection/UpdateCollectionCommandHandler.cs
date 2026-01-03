using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Collections.UpdateCollection;

/// <summary>
/// Handles updating collection details by processing UpdateCollectionCommand requests.
/// </summary>
public class UpdateCollectionCommandHandler(ISnippetDbContext dbContext) : IRequestHandler<UpdateCollectionCommand, ErrorOr<Unit>>
{

    /// <summary>
    /// Updates an existing collection's details.
    /// </summary>
    /// <param name="request">Command containing collection ID and new details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ErrorOr<Unit>> Handle(UpdateCollectionCommand request, CancellationToken cancellationToken)
    {
        var collection = await dbContext.Collections
            .FirstOrDefaultAsync(c => c.Id == new CollectionId(request.Id), cancellationToken);

        if (collection is null)
            return Error.Failure("Error", $"Collection with ID {request.Id} not found");

        collection.Rename(request.Name);
        collection.UpdateDescription(request.Description);
        collection.UpdateAppearance(request.Color, request.Icon);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
