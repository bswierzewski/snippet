using BuildingBlocks.Application.Models;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Commands.Collections.UpdateCollection;

/// <summary>
/// Command to update an existing collection's details.
/// </summary>
/// <param name="Id">Collection identifier.</param>
/// <param name="Name">New name for the collection.</param>
/// <param name="Description">New description for the collection.</param>
/// <param name="Color">New color in hexadecimal format.</param>
/// <param name="Icon">New icon name or emoji.</param>
public record UpdateCollectionCommand(
    Guid Id,
    string Name,
    string? Description,
    string? Color,
    string? Icon
) : IRequest<Result>;
