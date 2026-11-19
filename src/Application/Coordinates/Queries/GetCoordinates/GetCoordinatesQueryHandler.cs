using ApiGps.Application.Abstractions.Messaging;
using ApiGps.Application.Abstractions.Persistence;

namespace ApiGps.Application.Coordinates.Queries.GetCoordinates;

internal sealed class GetCoordinatesQueryHandler(ICoordinateReadRepository repository)
    : IQueryHandler<GetCoordinatesQuery, IReadOnlyList<CoordinateDto>>
{
    public Task<IReadOnlyList<CoordinateDto>> Handle(GetCoordinatesQuery query, CancellationToken cancellationToken) =>
        // Se recortan los espacios igual que al guardar (ClientCoordinate.Create).
        repository.ListByRouteAsync(query.Client.Trim(), query.RouteId.Trim(), cancellationToken);
}
