using BuildingBlocks.Application.Models;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.RemoveTag;

/// <summary>
/// Command to remove a tag from an existing snippet.
/// </summary>
/// <param name="SnippetId">Snippet identifier.</param>
/// <param name="TagId">Tag identifier to remove.</param>
public record RemoveTagCommand(
    Guid SnippetId,
    Guid TagId
) : IRequest<Result>;
