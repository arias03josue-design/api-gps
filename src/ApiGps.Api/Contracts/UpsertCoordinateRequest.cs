namespace ApiGps.Api.Contracts;

/// <summary>
/// Coordenada reportada por un cliente.
/// </summary>
/// <param name="Client">Identificador del cliente (obligatorio, máximo 100 caracteres).</param>
/// <param name="Coordinate">Coordenada en grados decimales con el formato "latitud,longitud". Ejemplo: "9.9281,-84.0907".</param>
/// <param name="CreatedAt">Fecha y hora ISO 8601 en que el cliente generó la coordenada. Ejemplo: "2026-11-18T15:30:00Z".</param>
public sealed record UpsertCoordinateRequest(string Client, string Coordinate, DateTimeOffset CreatedAt);
