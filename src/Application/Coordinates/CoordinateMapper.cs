using ApiGps.Domain.Coordinates;
using Riok.Mapperly.Abstractions;

namespace ApiGps.Application.Coordinates;

/// <summary>
/// Mapeos entre la entidad de dominio y el modelo de lectura, generados por Mapperly en
/// tiempo de compilación. Con <see cref="RequiredMappingStrategy.Both"/> cualquier propiedad
/// sin mapear produce un error de compilación, salvo que se ignore de forma explícita.
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Both)]
public static partial class CoordinateMapper
{
    /// <summary>
    /// Proyección para <see cref="IQueryable{T}"/>: EF Core la traduce a SQL y solo lee
    /// las columnas necesarias.
    /// </summary>
    public static partial IQueryable<CoordinateDto> ProjectToDto(this IQueryable<ClientCoordinate> query);

    [MapperIgnoreSource(nameof(ClientCoordinate.Id))]
    [MapperIgnoreSource(nameof(ClientCoordinate.ReceivedAt))]
    public static partial CoordinateDto ToDto(this ClientCoordinate coordinate);
}
