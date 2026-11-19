using Microsoft.AspNetCore.Authentication;

namespace ApiGps.Api.Security;

public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    /// <summary>Header que transporta la clave (configuración <c>ApiKey:HeaderName</c>).</summary>
    public string HeaderName { get; set; } = ApiKeyAuthenticationDefaults.DefaultHeaderName;

    /// <summary>Clave esperada (configuración <c>ApiKey:Value</c> / variable de entorno <c>ApiKey__Value</c>).</summary>
    public string Value { get; set; } = string.Empty;
}
