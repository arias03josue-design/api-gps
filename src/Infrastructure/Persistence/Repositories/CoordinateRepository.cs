using ApiGps.Application.Abstractions.Persistence;
using ApiGps.Domain.Coordinates;
using Microsoft.EntityFrameworkCore;

namespace ApiGps.Infrastructure.Persistence.Repositories;

internal sealed class CoordinateRepository(GpsDbContext dbContext) : ICoordinateRepository
{
    public Task UpsertAsync(ClientCoordinate coordinate, CancellationToken cancellationToken) =>
        // Upsert nativo de SQLite en una sola sentencia: cada posición nueva se agrega al historial y,
        // al ser atómico, los reintentos o peticiones concurrentes con el mismo cliente, ruta y
        // CreatedAt nunca generan duplicados (se actualiza la posición existente).
        // Los valores interpolados se envían como parámetros (no hay riesgo de inyección SQL).
        // Las fechas se envían como DateTime UTC, igual que las guarda UtcDateTimeOffsetConverter.
        dbContext.Database.ExecuteSqlAsync(
            $"""
             INSERT INTO "Coordinates" ("Client", "RouteId", "Coordinate", "CreatedAt", "ReceivedAt")
             VALUES ({coordinate.Client}, {coordinate.RouteId}, {coordinate.Coordinate}, {coordinate.CreatedAt.UtcDateTime}, {coordinate.ReceivedAt.UtcDateTime})
             ON CONFLICT ("Client", "RouteId", "CreatedAt") DO UPDATE SET
                 "Coordinate" = excluded."Coordinate",
                 "ReceivedAt" = excluded."ReceivedAt";
             """,
            cancellationToken);
}
