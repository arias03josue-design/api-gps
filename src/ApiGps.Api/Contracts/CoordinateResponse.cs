namespace ApiGps.Api.Contracts;

/// <summary>
/// Coordenada registrada.
/// </summary>
/// <param name="Client">Identificador del cliente.</param>
/// <param name="Coordinate">Coordenada con el formato "latitud,longitud".</param>
/// <param name="CreatedAt">Fecha y hora (UTC) en que el cliente generó la coordenada.</param>
public sealed record CoordinateResponse(string Client, string Coordinate, DateTimeOffset CreatedAt);
