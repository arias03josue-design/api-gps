using ApiGps.Application.Abstractions.Persistence;
using ApiGps.Application.Coordinates;
using Microsoft.EntityFrameworkCore;

namespace ApiGps.Infrastructure.Persistence.Repositories;

internal sealed class CoordinateReadRepository(GpsDbContext dbContext) : ICoordinateReadRepository
{
    public async Task<IReadOnlyList<CoordinateDto>> ListAsync(CancellationToken cancellationToken) =>
        await dbContext.Coordinates
            .AsNoTracking()
            .OrderByDescending(coordinate => coordinate.CreatedAt)
            .ThenBy(coordinate => coordinate.Client)
            .ProjectToDto() // Proyección generada por Mapperly, traducida a SQL por EF Core.
            .ToListAsync(cancellationToken);
}
