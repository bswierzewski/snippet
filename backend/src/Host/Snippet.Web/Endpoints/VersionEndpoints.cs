using BuildingBlocks.Application.Queries.BuildInfo;
using MediatR;

namespace Snippet.Web.Endpoints;

/// <summary>
/// Provides HTTP endpoints for version information.
/// </summary>
public static class VersionEndpoints
{
    /// <summary>
    /// Configures and maps all HTTP endpoints for version operations.
    /// </summary>
    public static IEndpointRouteBuilder MapVersionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api")
            .WithTags("Version")
            .WithOpenApi();
        // These endpoints are public, so we don't add .RequireAuthorization()

        group.MapGet("/version", GetVersion)
            .WithName("GetVersion")
            .WithSummary("Retrieves application build information")
            .Produces<BuildInfoDto>(StatusCodes.Status200OK);

        return endpoints;
    }

    private static async Task<IResult> GetVersion(IMediator mediator)
    {
        var result = await mediator.Send(new GetBuildInfoQuery());

        return result.IsSuccess
               ? Results.Ok(result.Value)
               : Results.Problem("Failed to retrieve build information");
    }
}
