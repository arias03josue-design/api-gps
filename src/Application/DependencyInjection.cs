using ApiGps.Application.Abstractions.Behaviors;
using ApiGps.Application.Abstractions.Messaging;
using ApiGps.Application.Abstractions.Validation;
using ApiGps.Application.Coordinates;
using ApiGps.Application.Coordinates.Commands.UpsertCoordinate;
using ApiGps.Application.Coordinates.Queries.GetCoordinates;
using ApiGps.Application.Routes.Commands.UpsertRoute;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ApiGps.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);

        // Comandos (escritura)
        services.AddSingleton<IValidator<UpsertCoordinateCommand>, UpsertCoordinateCommandValidator>();
        services.AddCommandHandler<UpsertCoordinateCommand, UpsertCoordinateCommandHandler>();

        services.AddSingleton<IValidator<UpsertRouteCommand>, UpsertRouteCommandValidator>();
        services.AddCommandHandler<UpsertRouteCommand, UpsertRouteCommandHandler>();

        // Consultas (lectura)
        services.AddSingleton<IValidator<GetCoordinatesQuery>, GetCoordinatesQueryValidator>();
        services.AddQueryHandler<GetCoordinatesQuery, IReadOnlyList<CoordinateDto>, GetCoordinatesQueryHandler>();

        return services;
    }

    /// <summary>
    /// Registra el manejador de un comando envuelto en el decorador de validación,
    /// de modo que todo comando se valida antes de ejecutarse.
    /// </summary>
    private static IServiceCollection AddCommandHandler<TCommand, THandler>(this IServiceCollection services)
        where TCommand : ICommand
        where THandler : class, ICommandHandler<TCommand>
    {
        services.AddScoped<THandler>();
        services.AddScoped<ICommandHandler<TCommand>>(provider => new ValidationCommandHandlerDecorator<TCommand>(
            provider.GetRequiredService<THandler>(),
            provider.GetServices<IValidator<TCommand>>()));

        return services;
    }

    /// <summary>
    /// Registra el manejador de una consulta envuelto en el decorador de validación,
    /// de modo que toda consulta se valida antes de ejecutarse.
    /// </summary>
    private static IServiceCollection AddQueryHandler<TQuery, TResult, THandler>(this IServiceCollection services)
        where TQuery : IQuery<TResult>
        where THandler : class, IQueryHandler<TQuery, TResult>
    {
        services.AddScoped<THandler>();
        services.AddScoped<IQueryHandler<TQuery, TResult>>(provider => new ValidationQueryHandlerDecorator<TQuery, TResult>(
            provider.GetRequiredService<THandler>(),
            provider.GetServices<IValidator<TQuery>>()));

        return services;
    }
}
