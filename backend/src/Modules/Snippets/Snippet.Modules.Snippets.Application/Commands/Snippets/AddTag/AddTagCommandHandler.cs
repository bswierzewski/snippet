using BuildingBlocks.Application.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.Aggregates;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.AddTag;

/// <summary>
/// Handles adding tags to snippets by processing AddTagCommand requests.
/// </summary>
public class AddTagCommandHandler : IRequestHandler<AddTagCommand, Result<Guid>>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;

    public AddTagCommandHandler(ISnippetsWriteDbContext writeDbContext)
    {
        _writeDbContext = writeDbContext;
    }

    /// <summary>
    /// Adds a tag to an existing snippet.
    /// </summary>
    /// <param name="request">Command containing snippet ID and tag details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the created tag.</returns>
    public async Task<Result<Guid>> Handle(AddTagCommand request, CancellationToken cancellationToken)
    {
        var snippet = await _writeDbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.SnippetId), cancellationToken);

        if (snippet is null)
            return Result<Guid>.Failure($"Snippet with ID {request.SnippetId} not found");

        var tagId = new TagId(Guid.NewGuid());
        var tag = new Tag(tagId, new SnippetId(request.SnippetId), request.TagName, request.Color);

        snippet.AddTag(tag);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(tagId.Value);
    }
}
