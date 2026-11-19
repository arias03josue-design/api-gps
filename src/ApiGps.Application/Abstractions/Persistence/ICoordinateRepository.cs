using ApiGps.Domain.Coordinates;

namespace ApiGps.Application.Abstractions.Persistence;

/// <summary>
/// Repositorio de escritura (lado de comandos) de las coordenadas.
/// </summary>
public interface ICoordinateRepository
{
    /// <summary>
    /// Inserta la coordenada o, si ya existe una del mismo cliente con el mismo <c>CreatedAt</c>,
    /// la reemplaza. La operación es atómica e idempotente.
    /// </summary>
    Task UpsertAsync(ClientCoordinate coordinate, CancellationToken cancellationToken);
}
