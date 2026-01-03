using BuildingBlocks.Abstractions.Extensions;
using ErrorOr;
using MediatR;

namespace Snippet.Modules.Snippets.Application.Queries.EnumValues;

/// <summary>
/// Handles retrieval of enum values by processing GetListEnumValuesQuery requests.
/// </summary>
public class GetListEnumValuesQueryHandler : IRequestHandler<GetListEnumValuesQuery, ErrorOr<IEnumerable<EnumValueDto>>>
{
    /// <summary>
    /// Retrieves all values for the specified enum type.
    /// </summary>
    /// <param name="request">Query request containing enum type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of enum values with their metadata.</returns>
    public Task<ErrorOr<IEnumerable<EnumValueDto>>> Handle(GetListEnumValuesQuery request, CancellationToken cancellationToken)
    {
        if (request.EnumType == null)
            return Task.FromResult<ErrorOr<IEnumerable<EnumValueDto>>>(Error.Validation("EnumType.Null", "Enum type cannot be null."));

        if (!request.EnumType.IsEnum)
            return Task.FromResult<ErrorOr<IEnumerable<EnumValueDto>>>(Error.Validation("EnumType.Invalid",
                $"Type '{request.EnumType.Name}' is not an enum type."));

        var enumValues = Enum.GetValues(request.EnumType)
            .Cast<Enum>()
            .Select(e => new EnumValueDto(
                Convert.ToInt32(e),
                e.ToString(),
                e.GetEnumDescription()
            ))
            .OrderBy(e => e.Name)
            .ToList();

        return Task.FromResult<ErrorOr<IEnumerable<EnumValueDto>>>(enumValues);
    }
}
