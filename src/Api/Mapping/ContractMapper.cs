using ApiGps.Api.Contracts;
using ApiGps.Application.Coordinates;
using ApiGps.Application.Coordinates.Commands.UpsertCoordinate;
using ApiGps.Application.Routes.Commands.UpsertRoute;
using Riok.Mapperly.Abstractions;

namespace ApiGps.Api.Mapping;

/// <summary>
/// Mapeos entre los contratos HTTP y los comandos/consultas de la capa de aplicación,
/// generados por Mapperly en tiempo de compilación (sin reflexión). Si un contrato y su
/// modelo dejan de coincidir, la compilación falla (ver .editorconfig: RMG012 y RMG020).
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Both)]
internal static partial class ContractMapper
{
    public static partial UpsertCoordinateCommand ToCommand(this UpsertCoordinateRequest request);

    public static partial UpsertRouteCommand ToCommand(this UpsertRouteRequest request);

    public static partial CoordinateResponse ToResponse(this CoordinateDto coordinate);

    public static partial IReadOnlyList<CoordinateResponse> ToResponses(this IReadOnlyList<CoordinateDto> coordinates);
}
