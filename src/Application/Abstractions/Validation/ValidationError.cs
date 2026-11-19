namespace ApiGps.Application.Abstractions.Validation;

/// <summary>
/// Error de validación asociado a una propiedad.
/// </summary>
public sealed record ValidationError(string PropertyName, string Message);
