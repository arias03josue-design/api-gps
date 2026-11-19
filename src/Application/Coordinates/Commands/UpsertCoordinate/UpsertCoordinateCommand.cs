using ApiGps.Application.Abstractions.Messaging;

namespace ApiGps.Application.Coordinates.Commands.UpsertCoordinate;

/// <summary>
/// Registra una posición del cliente dentro de una ruta. Si ya existe una posición del mismo
/// cliente, en la misma ruta y con el mismo <see cref="CreatedAt"/>, se actualiza
/// (semántica idempotente de PUT); en cualquier otro caso se agrega al historial.
/// </summary>
public sealed record UpsertCoordinateCommand(
    string Client,
    string RouteId,
    string Coordinate,
    DateTimeOffset CreatedAt) : ICommand;
