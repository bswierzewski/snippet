using BuildingBlocks.Application.Models;
using BuildingBlocks.Application.Queries.EnumValues;
using MediatR;
using Snippet.Modules.Snippets.Domain.Enums;

namespace Snippet.Web.Endpoints;

/// <summary>
/// Provides HTTP endpoints for lookup data operations such as enum values retrieval.
/// </summary>
public static class LookupDataEndpoints
{
    /// <summary>
    /// Configures and maps all HTTP endpoints for lookup data operations.
    /// </summary>
    public static IEndpointRouteBuilder MapLookupDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/lookup")
            .WithTags("LookupData")
            .WithOpenApi();
        // These endpoints are public, so we don't add .RequireAuthorization()

        // Enum endpoints
        group.MapGet("/enums/programming-languages", GetProgrammingLanguages)
            .WithName("GetProgrammingLanguageEnumValues")
            .WithSummary("Retrieves values for the ProgrammingLanguage enum")
            .Produces<IEnumerable<EnumValueDto>>(StatusCodes.Status200OK)
            .Produces<IReadOnlyCollection<Error>>(StatusCodes.Status400BadRequest);

        return endpoints;
    }

    private static async Task<IResult> GetProgrammingLanguages(IMediator mediator)
    {
        var query = new GetListEnumValuesQuery(typeof(ProgrammingLanguage));
        var result = await mediator.Send(query);
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }
}
