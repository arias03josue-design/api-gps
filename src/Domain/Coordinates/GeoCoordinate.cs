using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using ApiGps.Domain.Common;

namespace ApiGps.Domain.Coordinates;

/// <summary>
/// Objeto de valor que representa una coordenada geográfica en grados decimales
/// con el formato <c>"latitud,longitud"</c> (por ejemplo <c>"9.9281,-84.0907"</c>).
/// </summary>
public sealed record GeoCoordinate
{
    public const double MaxAbsoluteLatitude = 90;
    public const double MaxAbsoluteLongitude = 180;

    private const NumberStyles DegreesNumberStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;

    private GeoCoordinate(double latitude, double longitude, string value)
    {
        Latitude = latitude;
        Longitude = longitude;
        Value = value;
    }

    /// <summary>Latitud en grados decimales, entre -90 y 90.</summary>
    public double Latitude { get; }

    /// <summary>Longitud en grados decimales, entre -180 y 180.</summary>
    public double Longitude { get; }

    /// <summary>
    /// Representación normalizada <c>"latitud,longitud"</c>: conserva los dígitos enviados
    /// por el cliente y elimina los espacios alrededor de cada valor.
    /// </summary>
    public string Value { get; }

    /// <exception cref="DomainException">Si el texto no es una coordenada válida.</exception>
    public static GeoCoordinate Parse(string? value) =>
        TryParse(value, out var coordinate)
            ? coordinate
            : throw new DomainException(
                $"La coordenada '{value}' no es válida. Formato esperado: 'latitud,longitud' en grados decimales.");

    public static bool TryParse([NotNullWhen(true)] string? value, [NotNullWhen(true)] out GeoCoordinate? coordinate)
    {
        coordinate = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var parts = value.Split(',', StringSplitOptions.TrimEntries);

        if (parts.Length != 2
            || !TryParseDegrees(parts[0], MaxAbsoluteLatitude, out var latitude)
            || !TryParseDegrees(parts[1], MaxAbsoluteLongitude, out var longitude))
        {
            return false;
        }

        coordinate = new GeoCoordinate(latitude, longitude, $"{parts[0]},{parts[1]}");
        return true;
    }

    public override string ToString() => Value;

    private static bool TryParseDegrees(string text, double maxAbsolute, out double degrees) =>
        double.TryParse(text, DegreesNumberStyles, CultureInfo.InvariantCulture, out degrees)
        && double.IsFinite(degrees)
        && Math.Abs(degrees) <= maxAbsolute;
}
