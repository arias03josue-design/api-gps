using ApiGps.Domain.Common;
using ApiGps.Domain.Coordinates;

namespace ApiGps.Domain.Routes;

/// <summary>
/// Ruta que realiza un cliente y su estado actual. Hay una sola ruta por cliente e identificador
/// de ruta: reenviarla actualiza su estado en lugar de duplicarla.
/// </summary>
public sealed class ClientRoute
{
    public const int ClientMaxLength = ClientCoordinate.ClientMaxLength;
    public const int RouteIdMaxLength = ClientCoordinate.RouteIdMaxLength;

    // Constructor requerido por EF Core para materializar la entidad.
    private ClientRoute()
    {
    }

    private ClientRoute(string client, string routeId, RouteStatus status, DateTimeOffset updatedAt)
    {
        Client = client;
        RouteId = routeId;
        Status = status;
        UpdatedAt = updatedAt;
    }

    public long Id { get; private set; }

    /// <summary>Identificador del cliente que realiza la ruta.</summary>
    public string Client { get; private set; } = string.Empty;

    /// <summary>Identificador de la ruta (el mismo que usan sus posiciones).</summary>
    public string RouteId { get; private set; } = string.Empty;

    public RouteStatus Status { get; private set; }

    /// <summary>Instante (en UTC) en que el servidor recibió el estado por última vez.</summary>
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Crea una ruta válida. Los textos se recortan y la fecha se normaliza a UTC.
    /// </summary>
    /// <exception cref="DomainException">Si algún dato viola las reglas del dominio.</exception>
    public static ClientRoute Create(string client, string routeId, RouteStatus status, DateTimeOffset updatedAt)
    {
        var normalizedClient = RequiredText.Normalize(client, ClientMaxLength, "El cliente");
        var normalizedRouteId = RequiredText.Normalize(routeId, RouteIdMaxLength, "El identificador de la ruta");

        if (!Enum.IsDefined(status))
        {
            throw new DomainException($"El estado de la ruta no es válido. Valores posibles: {string.Join(", ", Enum.GetNames<RouteStatus>())}.");
        }

        return new ClientRoute(normalizedClient, normalizedRouteId, status, updatedAt.ToUniversalTime());
    }
}
