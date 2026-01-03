using ErrorOr;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Commands.Snippets.RecordUsage;

/// <summary>
/// Command to record usage of a snippet (e.g., when copied or viewed).
/// </summary>
/// <param name="SnippetId">Snippet identifier.</param>
public record RecordSnippetUsageCommand(Guid SnippetId) : IRequest<ErrorOr<Unit>>;
