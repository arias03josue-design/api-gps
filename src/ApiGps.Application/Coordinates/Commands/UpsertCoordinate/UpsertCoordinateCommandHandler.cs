using ApiGps.Application.Abstractions.Messaging;
using ApiGps.Application.Abstractions.Persistence;
using ApiGps.Domain.Coordinates;

namespace ApiGps.Application.Coordinates.Commands.UpsertCoordinate;

internal sealed class UpsertCoordinateCommandHandler(
    ICoordinateRepository repository,
    TimeProvider timeProvider) : ICommandHandler<UpsertCoordinateCommand>
{
    public Task Handle(UpsertCoordinateCommand command, CancellationToken cancellationToken)
    {
        // La entidad se crea mediante su fábrica para garantizar las invariantes del dominio.
        var coordinate = ClientCoordinate.Create(
            command.Client,
            command.Coordinate,
            command.CreatedAt,
            receivedAt: timeProvider.GetUtcNow());

        return repository.UpsertAsync(coordinate, cancellationToken);
    }
}
