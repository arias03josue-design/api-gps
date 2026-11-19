using ApiGps.Application.Abstractions.Messaging;

namespace ApiGps.Application.Coordinates.Commands.UpsertCoordinate;

/// <summary>
/// Registra la coordenada de un cliente. Si ya existe una coordenada del mismo cliente
/// con el mismo <see cref="CreatedAt"/>, se actualiza (semántica idempotente de PUT).
/// </summary>
public sealed record UpsertCoordinateCommand(string Client, string Coordinate, DateTimeOffset CreatedAt) : ICommand;
