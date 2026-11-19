using ApiGps.Domain.Routes;

namespace ApiGps.Application.Abstractions.Persistence;

/// <summary>
/// Repositorio de escritura (lado de comandos) de las rutas.
/// </summary>
public interface IRouteRepository
{
    /// <summary>
    /// Inserta la ruta o, si ya existe una del mismo cliente con el mismo identificador de ruta,
    /// actualiza su estado. La operación es atómica e idempotente.
    /// </summary>
    Task UpsertAsync(ClientRoute route, CancellationToken cancellationToken);
}
