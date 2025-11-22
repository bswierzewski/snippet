using Shared.Infrastructure.Models;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Commands.Tags.DeleteTag;

/// <summary>
/// Command to delete an existing tag.
/// </summary>
/// <param name="Id">Tag identifier to delete.</param>
public record DeleteTagCommand(Guid Id) : IRequest<Result>;
