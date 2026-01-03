using ErrorOr;
using MediatR;
using Snippet.Modules.Snippets.Application.Queries.Collections.GetCollectionById;

namespace Snippet.Modules.Snippets.Application.Queries.Collections.GetUserCollections;

/// <summary>
/// Query to retrieve all collections owned by the current user.
/// </summary>
public record GetUserCollectionsQuery() : IRequest<ErrorOr<IEnumerable<CollectionDto>>>;
