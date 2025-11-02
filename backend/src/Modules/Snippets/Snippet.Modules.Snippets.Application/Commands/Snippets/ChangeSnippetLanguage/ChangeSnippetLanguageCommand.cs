using BuildingBlocks.Application.Models;
using MediatR;
using Snippet.Modules.Snippets.Domain.Enums;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.ChangeSnippetLanguage;

/// <summary>
/// Command to change the programming language of an existing snippet.
/// </summary>
/// <param name="Id">Snippet identifier.</param>
/// <param name="Language">New programming language.</param>
public record ChangeSnippetLanguageCommand(
    Guid Id,
    ProgrammingLanguage Language
) : IRequest<Result>;
