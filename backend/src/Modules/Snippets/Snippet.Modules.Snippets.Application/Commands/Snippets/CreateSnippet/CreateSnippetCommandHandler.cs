using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Models;
using MediatR;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.CreateSnippet;

/// <summary>
/// Handles the creation of new snippets by processing CreateSnippetCommand requests.
/// </summary>
public class CreateSnippetCommandHandler : IRequestHandler<CreateSnippetCommand, Result<Guid>>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;
    private readonly IUser _user;

    public CreateSnippetCommandHandler(ISnippetsWriteDbContext writeDbContext, IUser user)
    {
        _writeDbContext = writeDbContext;
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
        var collectionIds = request.CollectionIds?
            .Select(id => new CollectionId(id))
            .ToList();

        var snippet = new Domain.Aggregates.Snippet(
            snippetId,
            _user.Id!.Value,
            request.Title,
            request.Content,
            request.Language,
            request.Description,
            collectionIds
        );

        await _writeDbContext.Snippets.AddAsync(snippet, cancellationToken);
        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(snippetId.Value);
    }
}
