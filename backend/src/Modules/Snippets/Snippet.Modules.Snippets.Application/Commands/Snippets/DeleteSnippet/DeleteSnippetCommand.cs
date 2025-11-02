using BuildingBlocks.Application.Models;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.DeleteSnippet;

/// <summary>
/// Command to delete an existing snippet.
/// </summary>
/// <param name="Id">Snippet identifier to delete.</param>
public record DeleteSnippetCommand(Guid Id) : IRequest<Result>;
