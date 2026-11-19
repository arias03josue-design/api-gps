using ApiGps.Api.Contracts;
using ApiGps.Api.Mapping;
using ApiGps.Application.Abstractions.Messaging;
using ApiGps.Application.Routes.Commands.UpsertRoute;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiGps.Api.Endpoints;

internal static class RouteEndpoints
{
    public static IEndpointRouteBuilder MapRouteEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(string.Empty)
            .WithTags("Rutas")
            .RequireAuthorization();

        // NSwag toma el resumen, la descripción y las respuestas de los comentarios XML de cada handler.
        group.MapPut("/route", UpsertRouteAsync)
            .WithName("UpsertRoute")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }

    /// <summary>Registra el estado de la ruta de un cliente.</summary>
    /// <remarks>
    /// Si ya existe una ruta con el mismo client y routeId, se actualiza su estado en lugar de duplicarse
    /// (por ejemplo, para pasarla de Activo a Completado). Los estados posibles son Activo y Completado.
    /// </remarks>
    /// <response code="200">Estado de la ruta registrado.</response>
    /// <response code="400">La solicitud no es válida (ver el detalle de "errors").</response>
    /// <response code="401">Falta el header ApiKey o su valor no es válido.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    private static async Task<Ok> UpsertRouteAsync(
        UpsertRouteRequest request,
        ICommandHandler<UpsertRouteCommand> handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(request.ToCommand(), cancellationToken);
        return TypedResults.Ok();
    }
}
