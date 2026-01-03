using ErrorOr;
using MediatR;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;

namespace Snippet.Modules.Snippets.Application.Queries.Snippets.GetFavoriteSnippets;

/// <summary>
/// Query to retrieve all snippets marked as favorite by the current user.
/// </summary>
public record GetFavoriteSnippetsQuery() : IRequest<ErrorOr<IEnumerable<SnippetSummaryDto>>>;
