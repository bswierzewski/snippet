using BuildingBlocks.Application.Models;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.ToggleFavorite;

/// <summary>
/// Command to toggle the favorite status of a snippet.
/// </summary>
/// <param name="SnippetId">Snippet identifier.</param>
public record ToggleSnippetFavoriteCommand(Guid SnippetId) : IRequest<Result>;
