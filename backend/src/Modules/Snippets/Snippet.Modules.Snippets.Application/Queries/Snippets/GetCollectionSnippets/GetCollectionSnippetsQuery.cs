using Shared.Infrastructure.Models;
using MediatR;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;

namespace Snippet.Modules.Snippets.Application.Queries.Snippets.GetCollectionSnippets;

/// <summary>
/// Query to retrieve all snippets within a specific collection.
/// </summary>
/// <param name="CollectionId">Collection unique identifier.</param>
public record GetCollectionSnippetsQuery(Guid CollectionId) : IRequest<Result<IEnumerable<SnippetSummaryDto>>>;
