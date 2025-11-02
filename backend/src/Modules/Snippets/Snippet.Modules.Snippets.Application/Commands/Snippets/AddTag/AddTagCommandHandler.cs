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
    /// Creates a new tag if it doesn't exist, or reuses an existing tag with the same name for the user.
    /// </summary>
    /// <param name="request">Command containing snippet ID and tag details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the tag (new or existing).</returns>
    public async Task<Result<Guid>> Handle(AddTagCommand request, CancellationToken cancellationToken)
    {
        var snippet = await _writeDbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == new SnippetId(request.SnippetId), cancellationToken);

        if (snippet is null)
            return Result<Guid>.Failure($"Snippet with ID {request.SnippetId} not found");

        // Check if a tag with the same name already exists for this user
        var existingTag = await _writeDbContext.Tags
            .FirstOrDefaultAsync(t => t.UserId == snippet.UserId && t.Name == request.TagName, cancellationToken);

        Tag tag;
        if (existingTag is not null)
        {
            // Reuse existing tag
            tag = existingTag;
        }
        else
        {
            // Create new tag
            var tagId = new TagId(Guid.NewGuid());
            tag = new Tag(tagId, snippet.UserId, request.TagName, request.Color);
            _writeDbContext.Tags.Add(tag);
        }

        // Assign tag to snippet (uses domain method that works with Tag object)
        snippet.AssignTag(tag);

        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(tag.Id.Value);
    }
}
