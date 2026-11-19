using ApiGps.Application.Coordinates;

namespace ApiGps.Application.Abstractions.Persistence;

/// <summary>
/// Repositorio de lectura (lado de consultas) de las coordenadas: devuelve directamente
/// modelos de lectura (DTO) proyectados desde la base de datos.
/// </summary>
public interface ICoordinateReadRepository
{
    /// <summary>
    /// Devuelve todas las coordenadas, de la más reciente a la más antigua según <c>CreatedAt</c>.
    /// </summary>
    Task<IReadOnlyList<CoordinateDto>> ListAsync(CancellationToken cancellationToken);
}
