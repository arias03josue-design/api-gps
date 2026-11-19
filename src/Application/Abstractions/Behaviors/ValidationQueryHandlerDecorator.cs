using ApiGps.Application.Abstractions.Messaging;
using ApiGps.Application.Abstractions.Validation;

namespace ApiGps.Application.Abstractions.Behaviors;

/// <summary>
/// Decorador que ejecuta todos los validadores registrados para la consulta antes de invocar
/// a su manejador. Si hay errores lanza <see cref="ValidationException"/> y el manejador no se ejecuta.
/// </summary>
internal sealed class ValidationQueryHandlerDecorator<TQuery, TResult>(
    IQueryHandler<TQuery, TResult> inner,
    IEnumerable<IValidator<TQuery>> validators) : IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public Task<TResult> Handle(TQuery query, CancellationToken cancellationToken)
    {
        validators.ValidateAndThrow(query);
        return inner.Handle(query, cancellationToken);
    }
}
