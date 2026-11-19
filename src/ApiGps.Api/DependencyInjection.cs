using ApiGps.Api.ErrorHandling;
using ApiGps.Api.OpenApi;
using ApiGps.Api.Security;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace ApiGps.Api;

internal static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // Ante un JSON inválido se lanza BadHttpRequestException para que GlobalExceptionHandler
        // responda con un ProblemDetails descriptivo (por defecto sería un 400 sin detalle).
        services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

        services.AddApiKeyAuthentication(configuration);
        services.AddAuthorization();

        services.AddEndpointsApiExplorer();
        services.AddNSwagDocument(configuration);

        return services;
    }

    private static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(ApiKeyAuthenticationDefaults.AuthenticationScheme)
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationDefaults.AuthenticationScheme,
                configureOptions: null);

        // La aplicación no arranca si la clave no está configurada (falla rápido y de forma explícita).
        services
            .AddOptions<ApiKeyAuthenticationOptions>(ApiKeyAuthenticationDefaults.AuthenticationScheme)
            .Bind(configuration.GetSection(ApiKeyAuthenticationDefaults.ConfigurationSection))
            .Validate(options => !string.IsNullOrWhiteSpace(options.HeaderName),
                "Debe configurar el header de la ApiKey en 'ApiKey:HeaderName'.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Value),
                "Debe configurar la ApiKey en 'ApiKey:Value' (variable de entorno ApiKey__Value).")
            .ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddNSwagDocument(this IServiceCollection services, IConfiguration configuration)
    {
        var headerName = configuration[$"{ApiKeyAuthenticationDefaults.ConfigurationSection}:HeaderName"];

        if (string.IsNullOrWhiteSpace(headerName))
        {
            headerName = ApiKeyAuthenticationDefaults.DefaultHeaderName;
        }

        services.AddOpenApiDocument(settings =>
        {
            settings.DocumentName = "v1";
            settings.Title = "API GPS";
            settings.Version = "v1";
            settings.Description = "API para registrar y consultar las coordenadas GPS reportadas por los clientes.";

            settings.AddSecurity(ApiKeyAuthenticationDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                In = OpenApiSecurityApiKeyLocation.Header,
                Name = headerName,
                Description = $"Clave de acceso enviada en el header '{headerName}'.",
            });

            // Marca como protegidos (candado en Swagger UI) los endpoints que requieren autorización.
            settings.OperationProcessors.Add(
                new AspNetCoreOperationSecurityScopeProcessor(ApiKeyAuthenticationDefaults.AuthenticationScheme));

            settings.SchemaSettings.SchemaProcessors.Add(new RequireNonNullablePropertiesSchemaProcessor());
        });

        return services;
    }
}
