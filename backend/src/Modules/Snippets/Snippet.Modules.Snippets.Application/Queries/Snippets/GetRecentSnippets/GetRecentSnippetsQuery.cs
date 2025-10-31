using BuildingBlocks.Application.Models;
using MediatR;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;

namespace Snippet.Modules.Snippets.Application.Queries.Snippets.GetRecentSnippets;

/// <summary>
/// Query to retrieve recently used snippets for the current user.
/// </summary>
/// <param name="Limit">Maximum number of snippets to retrieve.</param>
public record GetRecentSnippetsQuery(int Limit = 10) : IRequest<Result<IEnumerable<SnippetSummaryDto>>>;
