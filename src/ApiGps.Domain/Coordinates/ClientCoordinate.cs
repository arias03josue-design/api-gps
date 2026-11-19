using ApiGps.Domain.Common;

namespace ApiGps.Domain.Coordinates;

/// <summary>
/// Coordenada reportada por un cliente en un instante determinado.
/// Un cliente tiene como máximo una coordenada por cada <see cref="CreatedAt"/>,
/// lo que hace idempotente el registro (reenviar la misma lectura no la duplica).
/// </summary>
public sealed class ClientCoordinate
{
    public const int ClientMaxLength = 100;
    public const int CoordinateMaxLength = 64;

    // Constructor requerido por EF Core para materializar la entidad.
    private ClientCoordinate()
    {
    }

    private ClientCoordinate(string client, GeoCoordinate coordinate, DateTimeOffset createdAt, DateTimeOffset receivedAt)
    {
        Client = client;
        Coordinate = coordinate.Value;
        CreatedAt = createdAt;
        ReceivedAt = receivedAt;
    }

    public long Id { get; private set; }

    /// <summary>Identificador del cliente que reporta la coordenada.</summary>
    public string Client { get; private set; } = string.Empty;

    /// <summary>Coordenada normalizada con el formato <c>"latitud,longitud"</c>.</summary>
    public string Coordinate { get; private set; } = string.Empty;

    /// <summary>Instante (en UTC) en que el cliente generó la coordenada.</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>Instante (en UTC) en que el servidor recibió o actualizó la coordenada por última vez.</summary>
    public DateTimeOffset ReceivedAt { get; private set; }

    /// <summary>
    /// Crea una coordenada válida. Las fechas se normalizan a UTC.
    /// </summary>
    /// <exception cref="DomainException">Si algún dato viola las reglas del dominio.</exception>
    public static ClientCoordinate Create(string client, string coordinate, DateTimeOffset createdAt, DateTimeOffset receivedAt)
    {
        var normalizedClient = client?.Trim();

        if (string.IsNullOrEmpty(normalizedClient))
        {
            throw new DomainException("El cliente es obligatorio.");
        }

        if (normalizedClient.Length > ClientMaxLength)
        {
            throw new DomainException($"El cliente no puede superar los {ClientMaxLength} caracteres.");
        }

        var geoCoordinate = GeoCoordinate.Parse(coordinate);

        if (geoCoordinate.Value.Length > CoordinateMaxLength)
        {
            throw new DomainException($"La coordenada no puede superar los {CoordinateMaxLength} caracteres.");
        }

        if (createdAt == default)
        {
            throw new DomainException("La fecha de creación de la coordenada es obligatoria.");
        }

        return new ClientCoordinate(normalizedClient, geoCoordinate, createdAt.ToUniversalTime(), receivedAt.ToUniversalTime());
    }
}
