using ApiGps.Application.Abstractions.Messaging;
using ApiGps.Application.Abstractions.Persistence;
using ApiGps.Domain.Routes;

namespace ApiGps.Application.Routes.Commands.UpsertRoute;

internal sealed class UpsertRouteCommandHandler(
    IRouteRepository repository,
    TimeProvider timeProvider) : ICommandHandler<UpsertRouteCommand>
{
    public Task Handle(UpsertRouteCommand command, CancellationToken cancellationToken)
    {
        // La entidad se crea mediante su fábrica para garantizar las invariantes del dominio.
        var route = ClientRoute.Create(
            command.Client,
            command.RouteId,
            command.Status,
            updatedAt: timeProvider.GetUtcNow());

        return repository.UpsertAsync(route, cancellationToken);
    }
}
