using ApiGps.Application.Abstractions.Validation;
using ApiGps.Domain.Coordinates;

namespace ApiGps.Application.Coordinates.Commands.UpsertCoordinate;

/// <summary>
/// Valida el comando antes de llegar al manejador y reporta todos los errores a la vez.
/// Reutiliza las reglas del dominio (<see cref="GeoCoordinate"/> y los límites de <see cref="ClientCoordinate"/>).
/// </summary>
internal sealed class UpsertCoordinateCommandValidator : IValidator<UpsertCoordinateCommand>
{
    public IReadOnlyList<ValidationError> Validate(UpsertCoordinateCommand command)
    {
        var errors = new List<ValidationError>();

        if (string.IsNullOrWhiteSpace(command.Client))
        {
            errors.Add(new(nameof(command.Client), "El cliente es obligatorio."));
        }
        else if (command.Client.Trim().Length > ClientCoordinate.ClientMaxLength)
        {
            errors.Add(new(nameof(command.Client), $"El cliente no puede superar los {ClientCoordinate.ClientMaxLength} caracteres."));
        }

        if (string.IsNullOrWhiteSpace(command.Coordinate))
        {
            errors.Add(new(nameof(command.Coordinate), "La coordenada es obligatoria."));
        }
        else if (!GeoCoordinate.TryParse(command.Coordinate, out var coordinate))
        {
            errors.Add(new(
                nameof(command.Coordinate),
                "La coordenada debe tener el formato 'latitud,longitud' en grados decimales, con latitud entre -90 y 90 " +
                "y longitud entre -180 y 180. Ejemplo: '9.9281,-84.0907'."));
        }
        else if (coordinate.Value.Length > ClientCoordinate.CoordinateMaxLength)
        {
            errors.Add(new(nameof(command.Coordinate), $"La coordenada no puede superar los {ClientCoordinate.CoordinateMaxLength} caracteres."));
        }

        if (command.CreatedAt == default)
        {
            errors.Add(new(nameof(command.CreatedAt), "La fecha de creación (createdAt) es obligatoria."));
        }

        return errors;
    }
}
