using ApiGps.Application.Abstractions.Messaging;

namespace ApiGps.Application.Coordinates.Queries.GetCoordinates;

/// <summary>
/// Obtiene las posiciones de una ruta de un cliente, de la más antigua a la más reciente
/// según <c>CreatedAt</c>.
/// </summary>
public sealed record GetCoordinatesQuery(string Client, string RouteId) : IQuery<IReadOnlyList<CoordinateDto>>;
