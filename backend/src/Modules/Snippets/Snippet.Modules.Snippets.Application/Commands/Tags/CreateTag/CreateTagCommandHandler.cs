using Shared.Abstractions.Authorization;
using Shared.Infrastructure.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Snippet.Modules.Snippets.Application.Abstractions;
using Snippet.Modules.Snippets.Domain.Aggregates;
using Snippet.Modules.Snippets.Domain.ValueObjects;

namespace Snippet.Modules.Snippets.Application.Commands.Tags.CreateTag;

/// <summary>
/// Handles the creation of new tags by processing CreateTagCommand requests.
/// </summary>
public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Result<Guid>>
{
    private readonly ISnippetsWriteDbContext _writeDbContext;
    private readonly IUser _user;

    public CreateTagCommandHandler(ISnippetsWriteDbContext writeDbContext, IUser user)
    {
        _writeDbContext = writeDbContext;
        _user = user;
    }

    /// <summary>
    /// Creates a new tag entity and persists it to the database.
    /// Ensures the tag name is unique for the user (case-insensitive).
    /// </summary>
    /// <param name="request">Command containing tag details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the newly created tag.</returns>
    public async Task<Result<Guid>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        // Check if tag with the same name already exists for this user
        var normalizedName = request.Name.ToLowerInvariant();
        var existingTag = await _writeDbContext.Tags
            .FirstOrDefaultAsync(t => t.UserId == _user.Id!.Value && t.Name == normalizedName, cancellationToken);

        if (existingTag is not null)
            return Result<Guid>.Failure($"Tag with name '{normalizedName}' already exists");

        var tagId = new TagId(Guid.NewGuid());

        var tag = new Tag(
            tagId,
            _user.Id!.Value,
            request.Name,
            request.Color
        );

        await _writeDbContext.Tags.AddAsync(tag, cancellationToken);
        await _writeDbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(tagId.Value);
    }
}
