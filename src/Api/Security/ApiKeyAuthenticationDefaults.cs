namespace ApiGps.Api.Security;

public static class ApiKeyAuthenticationDefaults
{
    /// <summary>Nombre del esquema de autenticación.</summary>
    public const string AuthenticationScheme = "ApiKey";

    /// <summary>Sección de configuración (appsettings / variables de entorno <c>ApiKey__*</c>).</summary>
    public const string ConfigurationSection = "ApiKey";

    /// <summary>Header por defecto en el que los clientes envían la clave.</summary>
    public const string DefaultHeaderName = "ApiKey";
}
