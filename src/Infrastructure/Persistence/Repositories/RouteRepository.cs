using ApiGps.Application.Abstractions.Persistence;
using ApiGps.Domain.Routes;
using Microsoft.EntityFrameworkCore;

namespace ApiGps.Infrastructure.Persistence.Repositories;

internal sealed class RouteRepository(GpsDbContext dbContext) : IRouteRepository
{
    public Task UpsertAsync(ClientRoute route, CancellationToken cancellationToken) =>
        // Upsert nativo de SQLite en una sola sentencia: atómico, por lo que reintentos o peticiones
        // concurrentes con el mismo cliente y ruta nunca generan duplicados (se actualiza el estado).
        // Los valores interpolados se envían como parámetros (no hay riesgo de inyección SQL).
        // El estado se guarda con su nombre, igual que lo convierte ClientRouteConfiguration.
        dbContext.Database.ExecuteSqlAsync(
            $"""
             INSERT INTO "Routes" ("Client", "RouteId", "Status", "UpdatedAt")
             VALUES ({route.Client}, {route.RouteId}, {route.Status.ToString()}, {route.UpdatedAt.UtcDateTime})
             ON CONFLICT ("Client", "RouteId") DO UPDATE SET
                 "Status" = excluded."Status",
                 "UpdatedAt" = excluded."UpdatedAt";
             """,
            cancellationToken);
}
