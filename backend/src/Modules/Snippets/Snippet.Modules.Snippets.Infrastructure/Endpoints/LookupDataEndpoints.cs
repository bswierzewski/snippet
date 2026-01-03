using BuildingBlocks.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Snippet.Modules.Snippets.Application.Queries.EnumValues;
using Snippet.Modules.Snippets.Domain.Enums;

namespace Snippet.Modules.Snippets.Infrastructure.Endpoints;

/// <summary>
/// Provides HTTP endpoints for retrieving lookup data (enums, reference data, etc.) in the Snippet management system.
/// </summary>
public static class LookupDataEndpoints
{
    /// <summary>
    /// Configures and maps all HTTP endpoints for lookup data retrieval operations.
    /// </summary>
    public static void MapLookupDataEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/lookup")
            .WithTags("LookupData");
        // Public endpoints - no authorization required

        group.MapGet("/enums/programming-languages", GetProgrammingLanguageEnumValues)
            .WithName("GetProgrammingLanguageEnumValues")
            .WithSummary("Retrieves values for the ProgrammingLanguage enum")
            .Produces<IEnumerable<EnumValueDto>>(StatusCodes.Status200OK);
    }

    /// <summary>
    /// Retrieves all values for the ProgrammingLanguage enum.
    /// </summary>
    private static async Task<IResult> GetProgrammingLanguageEnumValues(ISender sender)
    {
        var query = new GetListEnumValuesQuery(typeof(ProgrammingLanguage));
        var result = await sender.Send(query);
        return result.ToHttpResult();
    }
}
