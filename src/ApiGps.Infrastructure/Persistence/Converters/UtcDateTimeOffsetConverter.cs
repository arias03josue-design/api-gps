using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ApiGps.Infrastructure.Persistence.Converters;

/// <summary>
/// Persiste un <see cref="DateTimeOffset"/> como <see cref="DateTime"/> en UTC y lo
/// recupera con desplazamiento +00:00.
/// </summary>
public sealed class UtcDateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTime>
{
    public UtcDateTimeOffsetConverter()
        : base(
            value => value.UtcDateTime,
            value => new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)))
    {
    }
}
