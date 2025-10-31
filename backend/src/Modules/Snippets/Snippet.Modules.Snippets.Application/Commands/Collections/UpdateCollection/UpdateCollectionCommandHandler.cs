using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Collections.UpdateCollection;

/// <summary>
/// Handles updating collection details by processing UpdateCollectionCommand requests.
/// </summary>
public class UpdateCollectionCommandHandler : IRequestHandler<UpdateCollectionCommand, Result>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;

    public UpdateCollectionCommandHandler(ISnippetsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Updates an existing collection's details.
    /// </summary>
    /// <param name="request">Command containing collection ID and new details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Result> Handle(UpdateCollectionCommand request, CancellationToken cancellationToken)
    {
        var collection = await _writeDbContext.Collections
            .FirstOrDefaultAsync(c => c.Id == new CollectionId(request.Id), cancellationToken);

        if (collection is null)
            return Result.Failure($"Collection with ID {request.Id} not found");

        collection.Rename(request.Name);
        collection.UpdateDescription(request.Description);
        collection.UpdateAppearance(request.Color, request.Icon);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
