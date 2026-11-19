using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ApiGps.Api.Security;

/// <summary>
/// Autentica las peticiones comparando el header configurado (por defecto <c>ApiKey</c>)
/// con la clave definida en la configuración.
/// </summary>
internal sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IProblemDetailsService problemDetailsService)
    : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var providedKeys) || string.IsNullOrEmpty(providedKeys))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (providedKeys.Count != 1 || !KeysMatch(providedKeys.ToString(), Options.Value))
        {
            return Task.FromResult(AuthenticateResult.Fail("La ApiKey proporcionada no es válida."));
        }

        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, "api-key-client")], Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    /// <summary>
    /// Responde 401 con un ProblemDetails que explica por qué se rechazó la petición.
    /// </summary>
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var authenticateResult = await HandleAuthenticateOnceSafeAsync();

        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.Headers.WWWAuthenticate = $"{Scheme.Name} header=\"{Options.HeaderName}\"";

        await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = Context,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "No autorizado",
                Detail = authenticateResult.Failure?.Message
                    ?? $"Debe enviar una ApiKey válida en el header '{Options.HeaderName}'.",
            },
        });
    }

    /// <summary>
    /// Comparación en tiempo constante sobre los hash SHA-256 para no filtrar
    /// información de la clave (ni su longitud) mediante ataques de temporización.
    /// </summary>
    private static bool KeysMatch(string providedKey, string expectedKey) =>
        CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(providedKey)),
            SHA256.HashData(Encoding.UTF8.GetBytes(expectedKey)));
}
