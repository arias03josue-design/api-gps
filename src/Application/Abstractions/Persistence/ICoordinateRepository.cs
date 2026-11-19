using ApiGps.Domain.Coordinates;

namespace ApiGps.Application.Abstractions.Persistence;

/// <summary>
/// Repositorio de escritura (lado de comandos) de las coordenadas.
/// </summary>
public interface ICoordinateRepository
{
    /// <summary>
    /// Agrega la posición al historial o, si ya existe una del mismo cliente, en la misma ruta y
    /// con el mismo <c>CreatedAt</c>, la reemplaza. La operación es atómica e idempotente.
    /// </summary>
    Task UpsertAsync(ClientCoordinate coordinate, CancellationToken cancellationToken);
}
