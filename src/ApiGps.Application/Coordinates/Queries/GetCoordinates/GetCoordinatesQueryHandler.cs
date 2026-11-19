using ApiGps.Application.Abstractions.Messaging;
using ApiGps.Application.Abstractions.Persistence;

namespace ApiGps.Application.Coordinates.Queries.GetCoordinates;

internal sealed class GetCoordinatesQueryHandler(ICoordinateReadRepository repository)
    : IQueryHandler<GetCoordinatesQuery, IReadOnlyList<CoordinateDto>>
{
    public Task<IReadOnlyList<CoordinateDto>> Handle(GetCoordinatesQuery query, CancellationToken cancellationToken) =>
        repository.ListAsync(cancellationToken);
}
