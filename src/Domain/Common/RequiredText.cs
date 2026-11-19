namespace ApiGps.Domain.Common;

/// <summary>
/// Normalización y validación compartida de los textos obligatorios del dominio.
/// </summary>
internal static class RequiredText
{
    /// <summary>
    /// Recorta el texto y verifica que no esté vacío ni supere la longitud máxima.
    /// </summary>
    /// <exception cref="DomainException">Si el texto está vacío o es demasiado largo.</exception>
    public static string Normalize(string? value, int maxLength, string displayName)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrEmpty(normalized))
        {
            throw new DomainException($"{displayName} es obligatorio.");
        }

        if (normalized.Length > maxLength)
        {
            throw new DomainException($"{displayName} no puede superar los {maxLength} caracteres.");
        }

        return normalized;
    }
}
