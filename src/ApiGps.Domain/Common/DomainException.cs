namespace ApiGps.Domain.Common;

/// <summary>
/// Se lanza cuando una operación viola una regla o invariante del dominio.
/// </summary>
public sealed class DomainException(string message) : Exception(message);
