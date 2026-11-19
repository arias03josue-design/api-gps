namespace ApiGps.Application.Abstractions.Validation;

/// <summary>
/// Valida una instancia de <typeparamref name="T"/> y devuelve los errores encontrados (vacío si es válida).
/// </summary>
public interface IValidator<in T>
{
    IReadOnlyList<ValidationError> Validate(T instance);
}
