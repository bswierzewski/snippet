using BuildingBlocks.Abstractions.Abstractions;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.Aggregates;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Tags.CreateTag;

/// <summary>
/// Handles the creation of new tags by processing CreateTagCommand requests.
/// </summary>
public class CreateTagCommandHandler(ISnippetDbContext dbContext, IUserContext user) : IRequestHandler<CreateTagCommand, ErrorOr<Guid>>
{

    /// <summary>
    /// Creates a new tag entity and persists it to the database.
    /// Ensures the tag name is unique for the user (case-insensitive).
    /// </summary>
    /// <param name="request">Command containing tag details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created tag.</returns>
    public async Task<ErrorOr<Guid>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        // Check if tag with the same name already exists for this user
        var normalizedName = request.Name.ToLowerInvariant();
        var existingTag = await dbContext.Tags
            .FirstOrDefaultAsync(t => t.UserId == user.Id && t.Name == normalizedName, cancellationToken);

        if (existingTag is not null)
            return Error.Failure("Tag.AlreadyExists", $"Tag with name '{normalizedName}' already exists");

        var tagId = new TagId(Guid.NewGuid());

        var tag = new Tag(
            tagId,
            user.Id,
            request.Name,
            request.Color
        );

        await dbContext.Tags.AddAsync(tag, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return tagId.Value;
    }
}
