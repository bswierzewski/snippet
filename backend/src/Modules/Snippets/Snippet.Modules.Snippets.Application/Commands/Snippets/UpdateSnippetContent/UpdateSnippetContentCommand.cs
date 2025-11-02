using BuildingBlocks.Application.Models;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.UpdateSnippetContent;

/// <summary>
/// Command to update the content of an existing snippet.
/// </summary>
/// <param name="Id">Snippet identifier.</param>
/// <param name="Content">New content for the snippet.</param>
public record UpdateSnippetContentCommand(
    Guid Id,
    string Content
) : IRequest<Result>;
