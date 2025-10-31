using BuildingBlocks.Application.Models;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.AddTag;

/// <summary>
/// Command to add a tag to an existing snippet.
/// </summary>
/// <param name="SnippetId">Snippet identifier.</param>
/// <param name="TagName">Name of the tag to add.</param>
/// <param name="Color">Optional color for the tag in hexadecimal format.</param>
public record AddTagCommand(
    Guid SnippetId,
    string TagName,
    string? Color
) : IRequest<Result<Guid>>;
