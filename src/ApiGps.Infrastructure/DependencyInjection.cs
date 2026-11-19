using ApiGps.Application.Abstractions.Persistence;
using ApiGps.Infrastructure.Persistence;
using ApiGps.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiGps.Infrastructure;

public static class DependencyInjection
{
    public const string ConnectionStringName = "Database";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"No se configuró la cadena de conexión 'ConnectionStrings:{ConnectionStringName}' " +
                $"(variable de entorno ConnectionStrings__{ConnectionStringName}).");
        }

        services.AddDbContext<GpsDbContext>(options => options.UseSqlite(connectionString));

        services.AddScoped<ICoordinateRepository, CoordinateRepository>();
        services.AddScoped<ICoordinateReadRepository, CoordinateReadRepository>();

        services.AddHealthChecks()
            .AddDbContextCheck<GpsDbContext>("database");

        return services;
    }
}
