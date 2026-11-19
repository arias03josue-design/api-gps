using ApiGps.Domain.Coordinates;

namespace ApiGps.Application.Abstractions.Validation;

/// <summary>
/// Reglas de validación compartidas por los comandos y las consultas que identifican
/// un cliente y una ruta, basadas en los límites que define el dominio.
/// </summary>
internal static class CommonValidationRules
{
    public static void ValidateClient(ICollection<ValidationError> errors, string? client) =>
        ValidateRequiredText(errors, nameof(ClientCoordinate.Client), client, ClientCoordinate.ClientMaxLength, "El cliente (client)");

    public static void ValidateRouteId(ICollection<ValidationError> errors, string? routeId) =>
        ValidateRequiredText(errors, nameof(ClientCoordinate.RouteId), routeId, ClientCoordinate.RouteIdMaxLength, "El identificador de la ruta (routeId)");

    private static void ValidateRequiredText(
        ICollection<ValidationError> errors,
        string propertyName,
        string? value,
        int maxLength,
        string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(new(propertyName, $"{displayName} es obligatorio."));
        }
        else if (value.Trim().Length > maxLength)
        {
            errors.Add(new(propertyName, $"{displayName} no puede superar los {maxLength} caracteres."));
        }
    }
}
