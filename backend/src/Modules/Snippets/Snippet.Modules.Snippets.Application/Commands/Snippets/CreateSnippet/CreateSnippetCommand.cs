using BuildingBlocks.Application.Models;
using MediatR;
using Snippet.Modules.Snippets.Domain.Enums;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.CreateSnippet;

/// <summary>
/// Command to create a new snippet with the provided details.
/// </summary>
/// <param name="Title">Snippet title.</param>
/// <param name="Content">Snippet content (code, query, or prompt text).</param>
/// <param name="Language">Programming language for syntax highlighting.</param>
/// <param name="Description">Optional description of the snippet.</param>
/// <param name="CollectionIds">Optional collection identifiers to organize the snippet.</param>
public record CreateSnippetCommand(
    string Title,
    string Content,
    ProgrammingLanguage Language,
    string? Description,
    List<Guid>? CollectionIds
) : IRequest<Result<Guid>>;
