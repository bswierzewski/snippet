using Shared.Infrastructure.Models;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Commands.Collections.CreateCollection;

/// <summary>
/// Command to create a new collection for organizing snippets.
/// </summary>
/// <param name="Name">Collection name.</param>
/// <param name="Description">Optional description of the collection.</param>
/// <param name="Color">Optional color in hexadecimal format.</param>
/// <param name="Icon">Optional icon name or emoji.</param>
public record CreateCollectionCommand(
    string Name,
    string? Description,
    string? Color,
    string? Icon
) : IRequest<Result<Guid>>;
