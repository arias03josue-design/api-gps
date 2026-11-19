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
        var errors = validators.SelectMany(validator => validator.Validate(command)).ToList();

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }

        return inner.Handle(command, cancellationToken);
    }
}
