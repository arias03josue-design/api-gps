namespace ApiGps.Domain.Routes;

/// <summary>
/// Estado de una ruta. Los valores empiezan en 1 para que el valor por defecto (0)
/// no sea un estado válido y un estado omitido se detecte como error.
/// </summary>
public enum RouteStatus
{
    /// <summary>La ruta está en curso.</summary>
    Activo = 1,

    /// <summary>La ruta ya finalizó.</summary>
    Completado = 2,
}
