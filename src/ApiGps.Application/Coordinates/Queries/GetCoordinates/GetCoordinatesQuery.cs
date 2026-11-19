using ApiGps.Application.Abstractions.Messaging;

namespace ApiGps.Application.Coordinates.Queries.GetCoordinates;

/// <summary>
/// Obtiene todas las coordenadas registradas.
/// </summary>
public sealed record GetCoordinatesQuery : IQuery<IReadOnlyList<CoordinateDto>>;
