namespace ApiGps.Application.Abstractions.Validation;

/// <summary>
/// Se lanza cuando un comando no supera la validación.
/// </summary>
public sealed class ValidationException(IReadOnlyList<ValidationError> errors)
    : Exception("La solicitud contiene uno o más errores de validación.")
{
    public IReadOnlyList<ValidationError> Errors { get; } = errors;
}
