using MediatR;
using Microsoft.AspNetCore.Mvc;
using Snippet.Modules.Snippets.Application.Commands.Snippets.AddTag;
using Snippet.Modules.Snippets.Application.Commands.Snippets.ChangeSnippetLanguage;
using Snippet.Modules.Snippets.Application.Commands.Snippets.CreateSnippet;
using Snippet.Modules.Snippets.Application.Commands.Snippets.DeleteSnippet;
using Snippet.Modules.Snippets.Application.Commands.Snippets.MoveSnippet;
using Snippet.Modules.Snippets.Application.Commands.Snippets.RecordUsage;
using Snippet.Modules.Snippets.Application.Commands.Snippets.RemoveTag;
using Snippet.Modules.Snippets.Application.Commands.Snippets.ToggleFavorite;
using Snippet.Modules.Snippets.Application.Commands.Snippets.UpdateSnippetContent;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetCollectionSnippets;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetFavoriteSnippets;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetRecentSnippets;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetSnippetById;
using Snippet.Modules.Snippets.Application.Queries.Snippets.GetUserSnippets;
using Snippet.Modules.Snippets.Application.Queries.Snippets.SearchSnippets;

namespace Snippet.Web.Endpoints;

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
            .WithOpenApi();

        group.MapPut("/{id:guid}/content", UpdateSnippetContent)
            .WithName("UpdateSnippetContent")
            .WithOpenApi();

        group.MapPut("/{id:guid}/language", ChangeSnippetLanguage)
            .WithName("ChangeSnippetLanguage")
            .WithOpenApi();

        group.MapPost("/{id:guid}/tags", AddTag)
            .WithName("AddTag")
            .WithOpenApi();

        group.MapDelete("/{id:guid}/tags/{tagId:guid}", RemoveTag)
            .WithName("RemoveTag")
            .WithOpenApi();

        group.MapPost("/{id:guid}/favorite", ToggleFavorite)
            .WithName("ToggleFavorite")
            .WithOpenApi();

        group.MapPost("/{id:guid}/usage", RecordUsage)
            .WithName("RecordUsage")
            .WithOpenApi();

        group.MapPut("/{id:guid}/collections", MoveSnippet)
            .WithName("MoveSnippet")
            .WithOpenApi();

        group.MapDelete("/{id:guid}", DeleteSnippet)
            .WithName("DeleteSnippet")
            .WithOpenApi();

        // Queries
        group.MapGet("/{id:guid}", GetSnippetById)
            .WithName("GetSnippetById")
            .WithOpenApi();

        group.MapGet("/", GetUserSnippets)
            .WithName("GetUserSnippets")
            .WithOpenApi();

        group.MapGet("/collections/{collectionId:guid}", GetCollectionSnippets)
            .WithName("GetCollectionSnippets")
            .WithOpenApi();

        group.MapGet("/favorites", GetFavoriteSnippets)
            .WithName("GetFavoriteSnippets")
            .WithOpenApi();

        group.MapGet("/recent", GetRecentSnippets)
            .WithName("GetRecentSnippets")
            .WithOpenApi();

        group.MapPost("/search", SearchSnippets)
            .WithName("SearchSnippets")
            .WithOpenApi();

        return endpoints;
    }

    // Command handlers
    private static async Task<IResult> CreateSnippet(
        [FromBody] CreateSnippetCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }

    private static async Task<IResult> UpdateSnippetContent(
        Guid id,
        [FromBody] UpdateSnippetContentCommand command,
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

    private static async Task<IResult> ChangeSnippetLanguage(
        Guid id,
        [FromBody] ChangeSnippetLanguageCommand command,
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

    private static async Task<IResult> AddTag(
        Guid id,
        [FromBody] AddTagCommand command,
        IMediator mediator)
    {
        if (id != command.SnippetId)
        {
            return Results.BadRequest("Route ID does not match command ID");
        }

        var result = await mediator.Send(command);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(result.Errors);
    }

    private static async Task<IResult> RemoveTag(
        Guid id,
        Guid tagId,
        IMediator mediator)
    {
        var result = await mediator.Send(new RemoveTagCommand(id, tagId));
        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(result.Errors);
    }

    private static async Task<IResult> ToggleFavorite(
        Guid id,
        IMediator mediator)
    {
        var result = await mediator.Send(new ToggleSnippetFavoriteCommand(id));
        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(result.Errors);
    }

    private static async Task<IResult> RecordUsage(
        Guid id,
        IMediator mediator)
    {
        var result = await mediator.Send(new RecordSnippetUsageCommand(id));
        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(result.Errors);
    }

    private static async Task<IResult> MoveSnippet(
        Guid id,
        [FromBody] MoveSnippetCommand command,
        IMediator mediator)
    {
        if (id != command.SnippetId)
        {
            return Results.BadRequest("Route ID does not match command ID");
        }

        var result = await mediator.Send(command);
        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(result.Errors);
    }

    private static async Task<IResult> DeleteSnippet(
        Guid id,
        IMediator mediator)
    {
        var result = await mediator.Send(new DeleteSnippetCommand(id));
        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(result.Errors);
    }

    // Query handlers
    private static async Task<IResult> GetSnippetById(
        Guid id,
        IMediator mediator)
    {
        var result = await mediator.Send(new GetSnippetByIdQuery(id));
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Errors);
    }

    private static async Task<IResult> GetUserSnippets(
        IMediator mediator)
    {
        var result = await mediator.Send(new GetUserSnippetsQuery());
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }

    private static async Task<IResult> GetCollectionSnippets(
        Guid collectionId,
        IMediator mediator)
    {
        var result = await mediator.Send(new GetCollectionSnippetsQuery(collectionId));
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Errors);
    }

    private static async Task<IResult> GetFavoriteSnippets(
        IMediator mediator)
    {
        var result = await mediator.Send(new GetFavoriteSnippetsQuery());
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }

    private static async Task<IResult> GetRecentSnippets(
        [FromQuery] int limit,
        IMediator mediator)
    {
        var result = await mediator.Send(new GetRecentSnippetsQuery(limit > 0 ? limit : 10));
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }

    private static async Task<IResult> SearchSnippets(
        [FromBody] SearchSnippetsQuery query,
        IMediator mediator)
    {
        var result = await mediator.Send(query);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }
}
