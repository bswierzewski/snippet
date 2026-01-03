using BuildingBlocks.Abstractions.Abstractions;
using ErrorOr;
using MediatR;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.Aggregates;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Collections.CreateCollection;

/// <summary>
/// Handles the creation of new collections by processing CreateCollectionCommand requests.
/// </summary>
public class CreateCollectionCommandHandler(ISnippetDbContext dbContext, IUserContext user) : IRequestHandler<CreateCollectionCommand, ErrorOr<Guid>>
{

    /// <summary>
    /// Creates a new collection entity and persists it to the database.
    /// </summary>
    /// <param name="request">Command containing collection details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created collection.</returns>
    public async Task<ErrorOr<Guid>> Handle(CreateCollectionCommand request, CancellationToken cancellationToken)
    {
        var collectionId = new CollectionId(Guid.NewGuid());

        var collection = new Collection(
            collectionId,
            user.Id,
            request.Name,
            request.Description,
            request.Color,
            request.Icon,
            sortOrder: 0
        );

        await dbContext.Collections.AddAsync(collection, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return collectionId.Value;
    }
}
