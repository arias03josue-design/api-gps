using ApiGps.Domain.Coordinates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiGps.Infrastructure.Persistence.Configurations;

internal sealed class ClientCoordinateConfiguration : IEntityTypeConfiguration<ClientCoordinate>
{
    // Si se cambian estos nombres, actualice también el SQL de CoordinateRepository.
    public const string TableName = "Coordinates";

    public void Configure(EntityTypeBuilder<ClientCoordinate> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(coordinate => coordinate.Id);

        builder.Property(coordinate => coordinate.Id)
            .ValueGeneratedOnAdd();

        builder.Property(coordinate => coordinate.Client)
            .HasMaxLength(ClientCoordinate.ClientMaxLength)
            .IsRequired();

        builder.Property(coordinate => coordinate.Coordinate)
            .HasMaxLength(ClientCoordinate.CoordinateMaxLength)
            .IsRequired();

        builder.Property(coordinate => coordinate.CreatedAt)
            .IsRequired();

        builder.Property(coordinate => coordinate.ReceivedAt)
            .IsRequired();

        // Una sola coordenada por cliente e instante: hace idempotente el PUT y es el
        // destino del "ON CONFLICT" del upsert.
        builder.HasIndex(coordinate => new { coordinate.Client, coordinate.CreatedAt })
            .IsUnique();

        // Acelera el listado ordenado por fecha.
        builder.HasIndex(coordinate => coordinate.CreatedAt);
    }
}
