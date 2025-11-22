using Shared.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Collections.DeleteCollection;

/// <summary>
/// Handles deletion of collections by processing DeleteCollectionCommand requests.
/// </summary>
public class DeleteCollectionCommandHandler : IRequestHandler<DeleteCollectionCommand, Result>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;

    public DeleteCollectionCommandHandler(ISnippetsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Deletes an existing collection from the database.
    /// </summary>
    /// <param name="request">Command containing collection ID to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<Result> Handle(DeleteCollectionCommand request, CancellationToken cancellationToken)
    {
        var collection = await _writeDbContext.Collections
            .FirstOrDefaultAsync(c => c.Id == new CollectionId(request.Id), cancellationToken);

        if (collection is null)
            return Result.Failure($"Collection with ID {request.Id} not found");

        _writeDbContext.Collections.Remove(collection);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
