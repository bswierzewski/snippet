using ErrorOr;
using MediatR;
using Snippet.Modules.Snippets.Domain.Enums;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.UpdateSnippet;

/// <summary>
/// Command to update an existing snippet.
/// </summary>
/// <param name="Id">Snippet identifier.</param>
/// <param name="Title">Title of the snippet.</param>
/// <param name="Description">Description of the snippet.</param>
/// <param name="Content">Content of the snippet.</param>
/// <param name="Language">Programming language for syntax highlighting.</param>
/// <param name="TagIds">List of tag IDs to assign to the snippet.</param>
/// <param name="CollectionIds">List of collection IDs to assign the snippet to.</param>
public record UpdateSnippetCommand(
    Guid Id,
    string Title,
    string? Description,
    string Content,
    ProgrammingLanguage Language,
    IEnumerable<Guid> TagIds,
    IEnumerable<Guid> CollectionIds
) : IRequest<ErrorOr<Unit>>;
