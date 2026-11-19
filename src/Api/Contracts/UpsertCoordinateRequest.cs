namespace ApiGps.Api.Contracts;

/// <summary>
/// Posición reportada por un cliente durante una ruta.
/// </summary>
/// <param name="Client">Identificador del cliente (obligatorio, máximo 100 caracteres).</param>
/// <param name="RouteId">Identificador de la ruta que está realizando el cliente (obligatorio, máximo 100 caracteres). Ejemplo: "ruta-2026-11-18-001".</param>
/// <param name="Coordinate">Coordenada en grados decimales con el formato "latitud,longitud". Ejemplo: "9.9281,-84.0907".</param>
/// <param name="CreatedAt">Fecha y hora ISO 8601 en que el cliente generó la posición. Ejemplo: "2026-11-18T15:30:00Z".</param>
public sealed record UpsertCoordinateRequest(string Client, string RouteId, string Coordinate, DateTimeOffset CreatedAt);
