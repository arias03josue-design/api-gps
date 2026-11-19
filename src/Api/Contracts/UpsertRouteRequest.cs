using ApiGps.Domain.Routes;

namespace ApiGps.Api.Contracts;

/// <summary>
/// Estado de la ruta de un cliente.
/// </summary>
/// <param name="RouteId">Identificador de la ruta (obligatorio, máximo 100 caracteres). Es el mismo que se envía en las posiciones. Ejemplo: "ruta-2026-11-18-001".</param>
/// <param name="Client">Identificador del cliente que realiza la ruta (obligatorio, máximo 100 caracteres).</param>
/// <param name="Status">Estado de la ruta: "Activo" o "Completado".</param>
public sealed record UpsertRouteRequest(string RouteId, string Client, RouteStatus Status);
