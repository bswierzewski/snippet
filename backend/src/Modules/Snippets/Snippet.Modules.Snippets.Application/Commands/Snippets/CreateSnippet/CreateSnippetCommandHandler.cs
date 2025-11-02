using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.CreateSnippet;

/// <summary>
/// Handles the creation of new snippets by processing CreateSnippetCommand requests.
/// </summary>
public class CreateSnippetCommandHandler : IRequestHandler<CreateSnippetCommand, Result<Guid>>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;
    private readonly ISnippetsReadDbContext _readDbContext;
    private readonly IUser _user;

    public CreateSnippetCommandHandler(
        ISnippetsWriteDbContext writeDbContext,
        ISnippetsReadDbContext readDbContext,
        IUser user)
    {
        _writeDbContext = writeDbContext;
        _readDbContext = readDbContext;
        _user = user;
    }

    /// <summary>
    /// Creates a new snippet entity and persists it to the database.
    /// </summary>
    /// <param name="request">Command containing snippet details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created snippet.</returns>
    public async Task<Result<Guid>> Handle(CreateSnippetCommand request, CancellationToken cancellationToken)
    {
        var snippetId = new SnippetId(Guid.NewGuid());

        // Fetch collections if provided
        List<Domain.Aggregates.Collection>? collections = null;
        if (request.CollectionIds is not null && request.CollectionIds.Any())
        {
            var collectionIdObjects = request.CollectionIds.Select(id => new CollectionId(id)).ToList();
            collections = await _readDbContext.Collections
                .Where(c => collectionIdObjects.Contains(c.Id))
                .ToListAsync(cancellationToken);
        }

        var snippet = new Domain.Aggregates.Snippet(
            snippetId,
            _user.Id!.Value,
            request.Title,
            request.Content,
            request.Language,
            request.Description,
            collections
        );

        await _writeDbContext.Snippets.AddAsync(snippet, cancellationToken);
        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(snippetId.Value);
    }
}
