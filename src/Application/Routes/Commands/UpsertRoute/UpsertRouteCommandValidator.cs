using ApiGps.Application.Abstractions.Validation;
using ApiGps.Domain.Routes;

namespace ApiGps.Application.Routes.Commands.UpsertRoute;

/// <summary>
/// Valida el comando antes de llegar al manejador y reporta todos los errores a la vez.
/// </summary>
internal sealed class UpsertRouteCommandValidator : IValidator<UpsertRouteCommand>
{
    public IReadOnlyList<ValidationError> Validate(UpsertRouteCommand command)
    {
        var errors = new List<ValidationError>();

        CommonValidationRules.ValidateRouteId(errors, command.RouteId);
        CommonValidationRules.ValidateClient(errors, command.Client);

        if (!Enum.IsDefined(command.Status))
        {
            errors.Add(new(
                nameof(command.Status),
                $"El estado (status) es obligatorio y debe ser uno de: {string.Join(", ", Enum.GetNames<RouteStatus>())}."));
        }

        return errors;
    }
}
