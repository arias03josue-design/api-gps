using ApiGps.Domain.Common;

namespace ApiGps.Domain.Coordinates;

/// <summary>
/// Posición reportada por un cliente durante una ruta. Se guardan todas las posiciones
/// (historial completo) para poder reconstruir el recorrido de cada ruta.
/// Dentro de una misma ruta, un cliente tiene como máximo una posición por cada
/// <see cref="CreatedAt"/>: reenviar la misma lectura la actualiza en lugar de duplicarla.
/// </summary>
public sealed class ClientCoordinate
{
    public const int ClientMaxLength = 100;
    public const int RouteIdMaxLength = 100;
    public const int CoordinateMaxLength = 64;

    // Constructor requerido por EF Core para materializar la entidad.
    private ClientCoordinate()
    {
    }

    private ClientCoordinate(
        string client,
        string routeId,
        GeoCoordinate coordinate,
        DateTimeOffset createdAt,
        DateTimeOffset receivedAt)
    {
        Client = client;
        RouteId = routeId;
        Coordinate = coordinate.Value;
        CreatedAt = createdAt;
        ReceivedAt = receivedAt;
    }

    public long Id { get; private set; }

    /// <summary>Identificador del cliente que reporta la posición.</summary>
    public string Client { get; private set; } = string.Empty;

    /// <summary>Identificador de la ruta que está realizando el cliente.</summary>
    public string RouteId { get; private set; } = string.Empty;

    /// <summary>Coordenada normalizada con el formato <c>"latitud,longitud"</c>.</summary>
    public string Coordinate { get; private set; } = string.Empty;

    /// <summary>Instante (en UTC) en que el cliente generó la posición.</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>Instante (en UTC) en que el servidor recibió o actualizó la posición por última vez.</summary>
    public DateTimeOffset ReceivedAt { get; private set; }

    /// <summary>
    /// Crea una posición válida. Los textos se recortan y las fechas se normalizan a UTC.
    /// </summary>
    /// <exception cref="DomainException">Si algún dato viola las reglas del dominio.</exception>
    public static ClientCoordinate Create(
        string client,
        string routeId,
        string coordinate,
        DateTimeOffset createdAt,
        DateTimeOffset receivedAt)
    {
        var normalizedClient = RequiredText.Normalize(client, ClientMaxLength, "El cliente");
        var normalizedRouteId = RequiredText.Normalize(routeId, RouteIdMaxLength, "El identificador de la ruta");

        var geoCoordinate = GeoCoordinate.Parse(coordinate);

        if (geoCoordinate.Value.Length > CoordinateMaxLength)
        {
            throw new DomainException($"La coordenada no puede superar los {CoordinateMaxLength} caracteres.");
        }

        if (createdAt == default)
        {
            throw new DomainException("La fecha de creación de la coordenada es obligatoria.");
        }

        return new ClientCoordinate(
            normalizedClient,
            normalizedRouteId,
            geoCoordinate,
            createdAt.ToUniversalTime(),
            receivedAt.ToUniversalTime());
    }
}
