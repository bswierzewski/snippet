using Shared.Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Snippet.Modules.Snippets.Application.Commands.Tags.CreateTag;
using Snippet.Modules.Snippets.Application.Commands.Tags.DeleteTag;
using Snippet.Modules.Snippets.Application.Queries.Tags.GetTags;
using Snippet.Modules.Snippets.Application.Queries.Tags.GetUserTags;

namespace Snippet.Web.Endpoints;

/// <summary>
/// Provides HTTP endpoints for tag management operations in the Snippet management system.
/// </summary>
public static class TagsEndpoints
{
    /// <summary>
    /// Configures and maps all HTTP endpoints for tag management operations.
    /// </summary>
    public static IEndpointRouteBuilder MapTagsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/tags")
            .WithTags("Tags")
            .RequireAuthorization();

        group.MapGet("/search", SearchTags)
            .WithName("SearchTags")
            .Produces<IEnumerable<TagSearchDto>>(StatusCodes.Status200OK)
            .Produces<IReadOnlyCollection<Error>>(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        group.MapGet("/", GetUserTags)
            .WithName("GetUserTags")
            .Produces<IEnumerable<TagDto>>(StatusCodes.Status200OK)
            .Produces<IReadOnlyCollection<Error>>(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        group.MapPost("/", CreateTag)
            .WithName("CreateTag")
            .Produces<Guid>(StatusCodes.Status200OK)
            .Produces<IReadOnlyCollection<Error>>(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        group.MapDelete("/{id:guid}", DeleteTag)
            .WithName("DeleteTag")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<IReadOnlyCollection<Error>>(StatusCodes.Status404NotFound)
            .WithOpenApi();

        return endpoints;
    }

    private static async Task<IResult> SearchTags(
        [FromQuery] string? searchTerm,
        IMediator mediator)
    {
        var result = await mediator.Send(new GetTagsQuery(searchTerm));
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }

    private static async Task<IResult> GetUserTags(
        IMediator mediator)
    {
        var result = await mediator.Send(new GetUserTagsQuery());
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }

    private static async Task<IResult> CreateTag(
        [FromBody] CreateTagCommand command,
        IMediator mediator)
    {
        var result = await mediator.Send(command);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }

    private static async Task<IResult> DeleteTag(
        Guid id,
        IMediator mediator)
    {
        var result = await mediator.Send(new DeleteTagCommand(id));
        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(result.Errors);
    }
}
