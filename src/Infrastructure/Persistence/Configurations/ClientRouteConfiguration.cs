using ApiGps.Domain.Routes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiGps.Infrastructure.Persistence.Configurations;

internal sealed class ClientRouteConfiguration : IEntityTypeConfiguration<ClientRoute>
{
    // Si se cambian estos nombres, actualice también el SQL de RouteRepository.
    public const string TableName = "Routes";

    public void Configure(EntityTypeBuilder<ClientRoute> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(route => route.Id);

        builder.Property(route => route.Id)
            .ValueGeneratedOnAdd();

        builder.Property(route => route.Client)
            .HasMaxLength(ClientRoute.ClientMaxLength)
            .IsRequired();

        builder.Property(route => route.RouteId)
            .HasMaxLength(ClientRoute.RouteIdMaxLength)
            .IsRequired();

        // Se guarda el nombre del estado ("Activo", "Completado") para que la base sea legible
        // y no dependa del valor numérico del enum.
        builder.Property(route => route.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(route => route.UpdatedAt)
            .IsRequired();

        // Una sola ruta por cliente e identificador: hace idempotente el PUT y es el destino
        // del "ON CONFLICT" del upsert.
        builder.HasIndex(route => new { route.Client, route.RouteId })
            .IsUnique();
    }
}
