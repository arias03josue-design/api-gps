using ApiGps.Application.Abstractions.Messaging;
using ApiGps.Application.Abstractions.Validation;

namespace ApiGps.Application.Abstractions.Behaviors;

/// <summary>
/// Decorador que ejecuta todos los validadores registrados para el comando antes de invocar
/// a su manejador. Si hay errores lanza <see cref="ValidationException"/> y el manejador no se ejecuta.
/// </summary>
internal sealed class ValidationCommandHandlerDecorator<TCommand>(
    ICommandHandler<TCommand> inner,
    IEnumerable<IValidator<TCommand>> validators) : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public Task Handle(TCommand command, CancellationToken cancellationToken)
    {
        validators.ValidateAndThrow(command);
        return inner.Handle(command, cancellationToken);
    }
}
