using ApiGps.Application.Coordinates;

namespace ApiGps.Application.Abstractions.Persistence;

/// <summary>
/// Repositorio de lectura (lado de consultas) de las coordenadas: devuelve directamente
/// modelos de lectura (DTO) proyectados desde la base de datos.
/// </summary>
public interface ICoordinateReadRepository
{
    /// <summary>
    /// Devuelve las posiciones de la ruta <paramref name="routeId"/> del cliente <paramref name="client"/>,
    /// ordenadas por <c>CreatedAt</c> de la más antigua a la más reciente. Si no hay, devuelve una lista vacía.
    /// </summary>
    Task<IReadOnlyList<CoordinateDto>> ListByRouteAsync(string client, string routeId, CancellationToken cancellationToken);
}
