using MediatR;
using Microsoft.AspNetCore.Mvc;
using Snippet.Modules.Snippets.Application.Commands.Collections.CreateCollection;
using Snippet.Modules.Snippets.Application.Commands.Collections.DeleteCollection;
using Snippet.Modules.Snippets.Application.Commands.Collections.UpdateCollection;
using Snippet.Modules.Snippets.Application.Queries.Collections.GetCollectionById;
using Snippet.Modules.Snippets.Application.Queries.Collections.GetUserCollections;

namespace Snippet.Web.Endpoints;

/// <summary>
/// Provides HTTP endpoints for collection management operations in the Snippet management system.
/// </summary>
public static class CollectionsEndpoints
{
    /// <summary>
    /// Configures and maps all HTTP endpoints for collection management operations.
    /// </summary>
    public static IEndpointRouteBuilder MapCollectionsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/collections")
            .WithTags("Collections")
            .RequireAuthorization();

        group.MapPost("/", CreateCollection)
            .WithName("CreateCollection")
            .WithOpenApi();

        group.MapGet("/{id:guid}", GetCollectionById)
            .WithName("GetCollectionById")
            .WithOpenApi();

        group.MapGet("/", GetUserCollections)
            .WithName("GetUserCollections")
            .WithOpenApi();

        group.MapPut("/{id:guid}", UpdateCollection)
            .WithName("UpdateCollection")
            .WithOpenApi();

        group.MapDelete("/{id:guid}", DeleteCollection)
            .WithName("DeleteCollection")
            .WithOpenApi();

        return endpoints;
    }

    private static async Task<IResult> CreateCollection(
        [FromBody] CreateCollectionCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }

    private static async Task<IResult> GetCollectionById(
        Guid id,
        IMediator mediator)
    {
        var result = await mediator.Send(new GetCollectionByIdQuery(id));
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Errors);
    }

    private static async Task<IResult> GetUserCollections(
        IMediator mediator)
    {
        var result = await mediator.Send(new GetUserCollectionsQuery());
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }

    private static async Task<IResult> UpdateCollection(
        Guid id,
        [FromBody] UpdateCollectionCommand command,
        IMediator mediator)
    {
        if (id != command.Id)
        {
            return Results.BadRequest("Route ID does not match command ID");
        }

        var result = await mediator.Send(command);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(result.Errors);
    }

    private static async Task<IResult> DeleteCollection(
        Guid id,
        IMediator mediator)
    {
        var result = await mediator.Send(new DeleteCollectionCommand(id));
        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(result.Errors);
    }
}
