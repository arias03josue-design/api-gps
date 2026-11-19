using System.Text.Json;
using ApiGps.Application.Abstractions.Validation;
using ApiGps.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ApiGps.Api.ErrorHandling;

/// <summary>
/// Convierte las excepciones en respuestas ProblemDetails (RFC 9457):
/// validación y reglas de dominio → 400, JSON inválido → 400, resto → 500.
/// </summary>
internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ValidationException validationException => new HttpValidationProblemDetails(ToErrorDictionary(validationException.Errors))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Error de validación",
                Detail = validationException.Message,
            },
            DomainException domainException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Regla de negocio no cumplida",
                Detail = domainException.Message,
            },
            BadHttpRequestException badRequestException => new ProblemDetails
            {
                Status = badRequestException.StatusCode,
                Title = "Solicitud inválida",
                Detail = badRequestException.InnerException is JsonException jsonException
                    ? $"El cuerpo de la solicitud no es un JSON válido o un campo tiene un formato incorrecto ({jsonException.Path ?? "$"})."
                    : badRequestException.Message,
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Error interno del servidor",
                Detail = "Ocurrió un error inesperado al procesar la solicitud.",
            },
        };

        if (problemDetails.Status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Error no controlado al procesar {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = problemDetails.Status!.Value;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception,
        });
    }

    /// <summary>
    /// Agrupa los errores por propiedad usando camelCase, igual que los nombres del JSON.
    /// </summary>
    private static Dictionary<string, string[]> ToErrorDictionary(IEnumerable<ValidationError> errors) =>
        errors
            .GroupBy(error => JsonNamingPolicy.CamelCase.ConvertName(error.PropertyName))
            .ToDictionary(group => group.Key, group => group.Select(error => error.Message).ToArray());
}
