namespace ApiGps.Application.Abstractions.Messaging;

/// <summary>
/// Procesa una consulta de tipo <typeparamref name="TQuery"/> y devuelve su resultado.
/// </summary>
public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken cancellationToken);
}
