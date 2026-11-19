using ApiGps.Api;
using ApiGps.Api.Endpoints;
using ApiGps.Application;
using ApiGps.Infrastructure;
using ApiGps.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPresentation(builder.Configuration);

var app = builder.Build();

// Crea la base de datos o aplica las migraciones pendientes, conservando los datos existentes.
await app.Services.InitializeDatabaseAsync();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Configuration.GetValue("Swagger:Enabled", defaultValue: true))
{
    app.UseOpenApi();                                     // /swagger/v1/swagger.json
    app.UseSwaggerUi(settings => settings.PersistAuthorization = true); // /swagger
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapCoordinateEndpoints();

await app.RunAsync();
