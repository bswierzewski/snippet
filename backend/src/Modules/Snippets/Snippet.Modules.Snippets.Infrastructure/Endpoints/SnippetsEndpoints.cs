using BuildingBlocks.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Snippet.Modules.Snippets.Application.Commands.Snippets.CreateSnippet;
using Snippet.Modules.Snippets.Application.Commands.Snippets.DeleteSnippet;
using Snippet.Modules.Snippets.Application.Commands.Snippets.RecordUsage;
using Snippet.Modules.Snippets.Application.Commands.Snippets.ToggleFavorite;
using Snippet.Modules.Snippets.Application.Commands.Snippets.UpdateSnippet;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetCollectionSnippets;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetFavoriteSnippets;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetRecentSnippets;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetSnippetById;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;
using Snippet.Modules.Snippets.Application.Queries.Snippets.SearchSnippets;

namespace Snippet.Modules.Snippets.Infrastructure.Endpoints;

/// <summary>
/// Provides HTTP endpoints for snippet management operations in the Snippet management system.
/// </summary>
public static class SnippetsEndpoints
{
    /// <summary>
    /// Configures and maps all HTTP endpoints for snippet management operations.
    /// </summary>
    public static IEndpointRouteBuilder MapSnippetsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/snippets")
            .WithTags("Snippets")
            .RequireAuthorization();

        // Commands
        group.MapPost("/", CreateSnippet)
            .WithName("CreateSnippet")
            .Produces<Guid>(StatusCodes.Status200OK);

        group.MapPut("/{id:guid}", UpdateSnippet)
            .WithName("UpdateSnippet")
            .Produces(StatusCodes.Status204NoContent);

        group.MapPost("/{id:guid}/favorite", ToggleFavorite)
            .WithName("ToggleFavorite")
            .Produces(StatusCodes.Status204NoContent);

        group.MapPost("/{id:guid}/usage", RecordUsage)
            .WithName("RecordUsage")
            .Produces(StatusCodes.Status204NoContent);

        group.MapDelete("/{id:guid}", DeleteSnippet)
            .WithName("DeleteSnippet")
            .Produces(StatusCodes.Status204NoContent);

        // Queries
        group.MapGet("/{id:guid}", GetSnippetById)
            .WithName("GetSnippetById")
            .Produces<GetSnippetByIdDto>(StatusCodes.Status200OK);

        group.MapGet("/", GetUserSnippets)
            .WithName("GetUserSnippets")
            .Produces<IEnumerable<SnippetSummaryDto>>(StatusCodes.Status200OK);

        group.MapGet("/collections/{collectionId:guid}", GetCollectionSnippets)
            .WithName("GetCollectionSnippets")
            .Produces<IEnumerable<SnippetSummaryDto>>(StatusCodes.Status200OK);

        group.MapGet("/favorites", GetFavoriteSnippets)
            .WithName("GetFavoriteSnippets")
            .Produces<IEnumerable<SnippetSummaryDto>>(StatusCodes.Status200OK);

        group.MapGet("/recent", GetRecentSnippets)
            .WithName("GetRecentSnippets")
            .Produces<IEnumerable<SnippetSummaryDto>>(StatusCodes.Status200OK);

        group.MapPost("/search", SearchSnippets)
            .WithName("SearchSnippets")
            .Produces<SearchSnippetsResponse>(StatusCodes.Status200OK);

        return endpoints;
    }

    // Command handlers
    private static async Task<IResult> CreateSnippet(
        [FromBody] CreateSnippetCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        return result.ToHttpResult();
    }

    private static async Task<IResult> UpdateSnippet(
        Guid id,
        [FromBody] UpdateSnippetCommand command,
        IMediator mediator)
    {
        if (id != command.Id)
        {
            return Results.BadRequest("Route ID does not match command ID");
        }

        var result = await mediator.Send(command);
        return result.ToNoContentResult();
    }

    private static async Task<IResult> ToggleFavorite(
        Guid id,
        IMediator mediator)
    {
        var result = await mediator.Send(new ToggleSnippetFavoriteCommand(id));
        return result.ToNoContentResult();
    }

    private static async Task<IResult> RecordUsage(
        Guid id,
        IMediator mediator)
    {
        var result = await mediator.Send(new RecordSnippetUsageCommand(id));
        return result.ToNoContentResult();
    }

    private static async Task<IResult> DeleteSnippet(
        Guid id,
        IMediator mediator)
    {
        var result = await mediator.Send(new DeleteSnippetCommand(id));
        return result.ToNoContentResult();
    }

    // Query handlers
    private static async Task<IResult> GetSnippetById(
        Guid id,
        IMediator mediator)
    {
        var result = await mediator.Send(new GetSnippetByIdQuery(id));
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetUserSnippets(
        IMediator mediator)
    {
        var result = await mediator.Send(new GetUserSnippetsQuery());
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetCollectionSnippets(
        Guid collectionId,
        IMediator mediator)
    {
        var result = await mediator.Send(new GetCollectionSnippetsQuery(collectionId));
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetFavoriteSnippets(
        IMediator mediator)
    {
        var result = await mediator.Send(new GetFavoriteSnippetsQuery());
        return result.ToHttpResult();
    }

    private static async Task<IResult> GetRecentSnippets(
        [FromQuery] int limit,
        IMediator mediator)
    {
        var result = await mediator.Send(new GetRecentSnippetsQuery(limit > 0 ? limit : 10));
        return result.ToHttpResult();
    }

    private static async Task<IResult> SearchSnippets(
        [FromBody] SearchSnippetsQuery query,
        IMediator mediator)
    {
        var result = await mediator.Send(query);
        return result.ToHttpResult();
    }
}
