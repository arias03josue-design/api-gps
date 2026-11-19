using ApiGps.Api.Contracts;
using ApiGps.Api.Mapping;
using ApiGps.Application.Abstractions.Messaging;
using ApiGps.Application.Coordinates;
using ApiGps.Application.Coordinates.Commands.UpsertCoordinate;
using ApiGps.Application.Coordinates.Queries.GetCoordinates;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ApiGps.Api.Endpoints;

internal static class CoordinateEndpoints
{
    public static IEndpointRouteBuilder MapCoordinateEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(string.Empty)
            .WithTags("Coordenadas")
            .RequireAuthorization();

        // NSwag toma el resumen, la descripción, los parámetros y las respuestas de los comentarios XML de cada handler.
        group.MapPut("/coordinate", UpsertCoordinateAsync)
            .WithName("UpsertCoordinate")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/coordinate", GetCoordinatesAsync)
            .WithName("GetCoordinates")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }

    /// <summary>Registra una posición del cliente dentro de una ruta.</summary>
    /// <remarks>
    /// Cada posición enviada se agrega al historial de la ruta indicada en routeId. La operación es
    /// idempotente: si se reenvía una posición con el mismo client, routeId y createdAt (mismo instante,
    /// aunque se envíe con otra zona horaria), se actualiza en lugar de duplicarse.
    /// </remarks>
    /// <response code="200">Posición registrada.</response>
    /// <response code="400">La solicitud no es válida (ver el detalle de "errors").</response>
    /// <response code="401">Falta el header ApiKey o su valor no es válido.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    private static async Task<Ok> UpsertCoordinateAsync(
        UpsertCoordinateRequest request,
        ICommandHandler<UpsertCoordinateCommand> handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(request.ToCommand(), cancellationToken);
        return TypedResults.Ok();
    }

    /// <summary>Lista las posiciones de una ruta de un cliente.</summary>
    /// <remarks>
    /// Devuelve las posiciones de la ruta indicada ordenadas por createdAt, de la más antigua a la más
    /// reciente. client y routeId son obligatorios y se comparan de forma exacta (distinguen mayúsculas
    /// y minúsculas). Si la ruta no tiene posiciones, devuelve una lista vacía.
    /// </remarks>
    /// <param name="client">Identificador del cliente.</param>
    /// <param name="routeId">Identificador de la ruta.</param>
    /// <param name="handler">Manejador de la consulta (se inyecta, no forma parte de la petición).</param>
    /// <param name="cancellationToken">Se cancela si el cliente aborta la petición.</param>
    /// <response code="200">Posiciones de la ruta (lista vacía si no hay).</response>
    /// <response code="400">Falta client o routeId, o superan los 100 caracteres.</response>
    /// <response code="401">Falta el header ApiKey o su valor no es válido.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    private static async Task<Ok<IReadOnlyList<CoordinateResponse>>> GetCoordinatesAsync(
        [FromQuery] string? client,
        [FromQuery] string? routeId,
        IQueryHandler<GetCoordinatesQuery, IReadOnlyList<CoordinateDto>> handler,
        CancellationToken cancellationToken)
    {
        // Se reciben como opcionales para que, si faltan, responda la validación de la capa de aplicación
        // con el mismo formato de errores que el PUT (y no el 400 genérico de ASP.NET Core).
        var query = new GetCoordinatesQuery(client ?? string.Empty, routeId ?? string.Empty);
        var coordinates = await handler.Handle(query, cancellationToken);
        return TypedResults.Ok(coordinates.ToResponses());
    }
}
