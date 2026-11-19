namespace ApiGps.Application.Abstractions.Messaging;

/// <summary>
/// CQRS - lado de lectura: una consulta obtiene datos sin modificar el estado del sistema.
/// </summary>
/// <typeparam name="TResult">Tipo del resultado de la consulta.</typeparam>
public interface IQuery<TResult>;
