using ApiGps.Application.Abstractions.Messaging;
using ApiGps.Domain.Routes;

namespace ApiGps.Application.Routes.Commands.UpsertRoute;

/// <summary>
/// Registra el estado de la ruta de un cliente. Si ya existe una ruta con el mismo
/// <see cref="Client"/> y <see cref="RouteId"/>, se actualiza su estado (semántica idempotente de PUT).
/// </summary>
public sealed record UpsertRouteCommand(string RouteId, string Client, RouteStatus Status) : ICommand;
