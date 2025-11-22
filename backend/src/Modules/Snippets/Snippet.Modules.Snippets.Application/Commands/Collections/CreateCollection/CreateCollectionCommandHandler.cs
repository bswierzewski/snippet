using Shared.Abstractions.Authorization;
using Shared.Infrastructure.Models;
using MediatR;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.Aggregates;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Collections.CreateCollection;

/// <summary>
/// Handles the creation of new collections by processing CreateCollectionCommand requests.
/// </summary>
public class CreateCollectionCommandHandler : IRequestHandler<CreateCollectionCommand, Result<Guid>>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;
    private readonly IUser _user;

    public CreateCollectionCommandHandler(ISnippetsWriteDbContext writeDbContext, IUser user)
    {
        _writeDbContext = writeDbContext;
        _user = user;
    }

    /// <summary>
    /// Creates a new collection entity and persists it to the database.
    /// </summary>
    /// <param name="request">Command containing collection details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created collection.</returns>
    public async Task<Result<Guid>> Handle(CreateCollectionCommand request, CancellationToken cancellationToken)
    {
        var collectionId = new CollectionId(Guid.NewGuid());

        var collection = new Collection(
            collectionId,
            _user.Id!.Value,
            request.Name,
            request.Description,
            request.Color,
            request.Icon,
            sortOrder: 0
        );

        await _writeDbContext.Collections.AddAsync(collection, cancellationToken);
        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(collectionId.Value);
    }
}
