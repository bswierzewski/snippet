using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.UpdateSnippet;

/// <summary>
/// Handles updating snippet by processing UpdateSnippetCommand requests.
/// </summary>
public class UpdateSnippetCommandHandler(ISnippetDbContext dbContext) : IRequestHandler<UpdateSnippetCommand, ErrorOr<Unit>>
{

    /// <summary>
    /// Updates an existing snippet with all provided data.
    /// </summary>
    /// <param name="request">Command containing snippet data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ErrorOr<Unit>> Handle(UpdateSnippetCommand request, CancellationToken cancellationToken)
    {
        var snippet = await dbContext.Snippets
            .Include(s => s.SnippetTags)
            .Include(s => s.SnippetCollections)
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.Id), cancellationToken);

        if (snippet is null)
            return Error.Failure("Error", $"Snippet with ID {request.Id} not found");

        // Update basic properties
        snippet.Update(request.Title, request.Description, request.Content, request.Language);

        // Update tags
        var tagIdObjects = request.TagIds.Select(id => new TagId(id)).ToList();
        var tags = await dbContext.Tags
            .AsNoTracking()
            .Where(t => tagIdObjects.Contains(t.Id))
            .ToListAsync(cancellationToken);

        snippet.UpdateTags(tags);

        // Update collections
        var collectionIdObjects = request.CollectionIds.Select(id => new CollectionId(id)).ToList();
        var collections = await dbContext.Collections
            .AsNoTracking()
            .Where(c => collectionIdObjects.Contains(c.Id))
            .ToListAsync(cancellationToken);

        snippet.UpdateCollections(collections);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
