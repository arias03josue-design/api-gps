using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ApiGps.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    /// <summary>
    /// Crea la base de datos si no existe y aplica las migraciones pendientes.
    /// Nunca elimina datos: si el archivo ya existe (por ejemplo, en un volumen de Docker),
    /// solo se actualiza su esquema.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GpsDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(DatabaseInitializer).FullName!);

        var dataSource = new SqliteConnectionStringBuilder(dbContext.Database.GetConnectionString()).DataSource;

        if (!string.IsNullOrWhiteSpace(dataSource) && dataSource != ":memory:")
        {
            dataSource = Path.GetFullPath(dataSource);

            // SQLite crea el archivo de la base de datos, pero no el directorio que lo contiene.
            Directory.CreateDirectory(Path.GetDirectoryName(dataSource)!);
        }

        logger.LogInformation("Aplicando migraciones a la base de datos SQLite: {DataSource}", dataSource);
        await dbContext.Database.MigrateAsync(cancellationToken);

        // WAL permite leer mientras se escribe y es más resistente a cierres inesperados.
        // El modo queda guardado en el propio archivo de la base de datos.
        await dbContext.Database.ExecuteSqlRawAsync("PRAGMA journal_mode = WAL;", cancellationToken);
    }
}
