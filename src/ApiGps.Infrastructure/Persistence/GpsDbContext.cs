using ApiGps.Domain.Coordinates;
using ApiGps.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;

namespace ApiGps.Infrastructure.Persistence;

public sealed class GpsDbContext(DbContextOptions<GpsDbContext> options) : DbContext(options)
{
    public DbSet<ClientCoordinate> Coordinates => Set<ClientCoordinate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GpsDbContext).Assembly);

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // SQLite no tiene un tipo nativo para DateTimeOffset y EF Core no puede ordenar ni comparar
        // ese tipo en SQL. Se guarda como fecha UTC en texto ISO-8601, que sí se ordena correctamente.
        configurationBuilder.Properties<DateTimeOffset>().HaveConversion<UtcDateTimeOffsetConverter>();
    }
}
