namespace ApiGps.Application.Coordinates;

/// <summary>
/// Modelo de lectura de una posición registrada.
/// </summary>
public sealed record CoordinateDto(string Client, string RouteId, string Coordinate, DateTimeOffset CreatedAt);
