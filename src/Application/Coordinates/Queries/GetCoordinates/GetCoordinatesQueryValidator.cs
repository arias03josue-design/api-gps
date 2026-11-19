using ApiGps.Application.Abstractions.Validation;

namespace ApiGps.Application.Coordinates.Queries.GetCoordinates;

/// <summary>
/// Exige el cliente y la ruta que se quieren consultar.
/// </summary>
internal sealed class GetCoordinatesQueryValidator : IValidator<GetCoordinatesQuery>
{
    public IReadOnlyList<ValidationError> Validate(GetCoordinatesQuery query)
    {
        var errors = new List<ValidationError>();

        CommonValidationRules.ValidateClient(errors, query.Client);
        CommonValidationRules.ValidateRouteId(errors, query.RouteId);

        return errors;
    }
}
