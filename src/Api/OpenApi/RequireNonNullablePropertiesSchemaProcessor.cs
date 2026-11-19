using NJsonSchema;
using NJsonSchema.Generation;

namespace ApiGps.Api.OpenApi;

/// <summary>
/// Marca como obligatorias ("required") en el documento OpenAPI las propiedades que no aceptan
/// null según las anotaciones de nulabilidad de C#. Así Swagger UI las muestra como requeridas y
/// los clientes generados con NSwag (C#/TypeScript) no las declaran como opcionales.
/// </summary>
internal sealed class RequireNonNullablePropertiesSchemaProcessor : ISchemaProcessor
{
    public void Process(SchemaProcessorContext context)
    {
        foreach (var property in context.Schema.Properties.Values)
        {
            if (!property.IsNullable(SchemaType.OpenApi3))
            {
                property.IsRequired = true;
            }
        }
    }
}
