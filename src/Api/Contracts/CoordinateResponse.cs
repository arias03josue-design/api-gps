namespace ApiGps.Api.Contracts;

/// <summary>
/// Posición registrada.
/// </summary>
/// <param name="Client">Identificador del cliente.</param>
/// <param name="RouteId">Identificador de la ruta a la que pertenece la posición.</param>
/// <param name="Coordinate">Coordenada con el formato "latitud,longitud".</param>
/// <param name="CreatedAt">Fecha y hora (UTC) en que el cliente generó la posición.</param>
public sealed record CoordinateResponse(string Client, string RouteId, string Coordinate, DateTimeOffset CreatedAt);
