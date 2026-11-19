using ApiGps.Application.Abstractions.Persistence;
using ApiGps.Application.Coordinates;
using Microsoft.EntityFrameworkCore;

namespace ApiGps.Infrastructure.Persistence.Repositories;

internal sealed class CoordinateReadRepository(GpsDbContext dbContext) : ICoordinateReadRepository
{
    public async Task<IReadOnlyList<CoordinateDto>> ListByRouteAsync(
        string client,
        string routeId,
        CancellationToken cancellationToken) =>
        await dbContext.Coordinates
            .AsNoTracking()
            .Where(coordinate => coordinate.Client == client && coordinate.RouteId == routeId)
            // De la más antigua a la más reciente. Id (orden de llegada) desempata las posiciones con
            // el mismo instante. El índice único (Client, RouteId, CreatedAt) resuelve el filtro y el orden.
            .OrderBy(coordinate => coordinate.CreatedAt)
            .ThenBy(coordinate => coordinate.Id)
            .ProjectToDto() // Proyección generada por Mapperly, traducida a SQL por EF Core.
            .ToListAsync(cancellationToken);
}
