namespace ApiGps.Application.Coordinates;

/// <summary>
/// Modelo de lectura de una coordenada.
/// </summary>
public sealed record CoordinateDto(string Client, string Coordinate, DateTimeOffset CreatedAt);
