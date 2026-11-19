using ApiGps.Api.Contracts;
using ApiGps.Api.Mapping;
using ApiGps.Application.Abstractions.Messaging;
using ApiGps.Application.Coordinates;
using ApiGps.Application.Coordinates.Commands.UpsertCoordinate;
using ApiGps.Application.Coordinates.Queries.GetCoordinates;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApiGps.Api.Endpoints;

internal static class CoordinateEndpoints
{
    public static IEndpointRouteBuilder MapCoordinateEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(string.Empty)
            .WithTags("Coordenadas")
            .RequireAuthorization();

        // NSwag toma el resumen, la descripción y las respuestas de los comentarios XML de cada handler.
        group.MapPut("/coordinate", UpsertCoordinateAsync)
            .WithName("UpsertCoordinate")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/coordenadas", GetCoordinatesAsync)
            .WithName("GetCoordinates")
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return app;
    }

    /// <summary>Registra la coordenada de un cliente.</summary>
    /// <remarks>
    /// La operación es idempotente: si ya existe una coordenada del mismo cliente con el mismo
    /// createdAt (mismo instante, aunque se envíe con otra zona horaria), se reemplaza en lugar de duplicarse.
    /// </remarks>
    /// <response code="200">Coordenada registrada.</response>
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

    /// <summary>Lista las coordenadas registradas.</summary>
    /// <remarks>Devuelve todas las coordenadas, de la más reciente a la más antigua según createdAt.</remarks>
    /// <response code="200">Listado de coordenadas.</response>
    /// <response code="401">Falta el header ApiKey o su valor no es válido.</response>
    /// <response code="500">Error inesperado del servidor.</response>
    private static async Task<Ok<IReadOnlyList<CoordinateResponse>>> GetCoordinatesAsync(
        IQueryHandler<GetCoordinatesQuery, IReadOnlyList<CoordinateDto>> handler,
        CancellationToken cancellationToken)
    {
        var coordinates = await handler.Handle(new GetCoordinatesQuery(), cancellationToken);
        return TypedResults.Ok(coordinates.ToResponses());
    }
}
