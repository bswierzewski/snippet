using BuildingBlocks.Application.Models;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Commands.Tags.CreateTag;

/// <summary>
/// Command to create a new tag for categorizing snippets.
/// </summary>
/// <param name="Name">Tag name (will be stored in lowercase).</param>
/// <param name="Color">Optional color in hexadecimal format.</param>
public record CreateTagCommand(
    string Name,
    string? Color
) : IRequest<Result<Guid>>;
