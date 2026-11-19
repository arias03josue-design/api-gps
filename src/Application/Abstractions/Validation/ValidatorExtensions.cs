namespace ApiGps.Application.Abstractions.Validation;

internal static class ValidatorExtensions
{
    /// <summary>
    /// Ejecuta todos los validadores y, si alguno falla, lanza <see cref="ValidationException"/>
    /// con el conjunto completo de errores.
    /// </summary>
    public static void ValidateAndThrow<T>(this IEnumerable<IValidator<T>> validators, T instance)
    {
        var errors = validators.SelectMany(validator => validator.Validate(instance)).ToList();

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
    }
}
