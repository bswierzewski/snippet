using BuildingBlocks.Application.Models;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.MoveSnippet;

/// <summary>
/// Command to update the collections a snippet belongs to.
/// </summary>
/// <param name="SnippetId">Snippet identifier.</param>
/// <param name="CollectionIds">Collection identifiers the snippet should belong to.</param>
public record MoveSnippetCommand(
    Guid SnippetId,
    List<Guid> CollectionIds
) : IRequest<Result>;
