using ErrorOr;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Commands.Collections.DeleteCollection;

/// <summary>
/// Command to delete an existing collection.
/// </summary>
/// <param name="Id">Collection identifier to delete.</param>
public record DeleteCollectionCommand(Guid Id) : IRequest<ErrorOr<Unit>>;
