namespace ApiGps.Application.Abstractions.Messaging;

/// <summary>
/// Procesa un comando de tipo <typeparamref name="TCommand"/>.
/// </summary>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task Handle(TCommand command, CancellationToken cancellationToken);
}
