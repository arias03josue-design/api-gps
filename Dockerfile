# syntax=docker/dockerfile:1

# ============================================================
# Etapa 1: compilación y publicación con el SDK de .NET 10
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Primero solo los archivos de proyecto: la restauración de paquetes queda en caché
# y solo se repite cuando cambian las dependencias.
COPY .editorconfig Directory.Build.props Directory.Packages.props ./
COPY src/ApiGps.Domain/ApiGps.Domain.csproj src/ApiGps.Domain/
COPY src/ApiGps.Application/ApiGps.Application.csproj src/ApiGps.Application/
COPY src/ApiGps.Infrastructure/ApiGps.Infrastructure.csproj src/ApiGps.Infrastructure/
COPY src/ApiGps.Api/ApiGps.Api.csproj src/ApiGps.Api/
RUN dotnet restore src/ApiGps.Api/ApiGps.Api.csproj

COPY src/ src/
RUN dotnet publish src/ApiGps.Api/ApiGps.Api.csproj \
        --configuration "$BUILD_CONFIGURATION" \
        --no-restore \
        --output /app/publish \
        -p:UseAppHost=false

# ============================================================
# Etapa 2: imagen final, solo con el runtime de ASP.NET Core
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Carpeta de la base de datos SQLite. docker-compose monta sobre ella un volumen con nombre,
# por lo que los datos sobreviven aunque el contenedor se elimine o se recree con otra imagen.
# Pertenece al usuario sin privilegios "app" (APP_UID) para que pueda escribir en ella.
RUN mkdir -p /app/data && chown "$APP_UID:$APP_UID" /app/data

ENV ASPNETCORE_HTTP_PORTS=8080 \
    ConnectionStrings__Database="Data Source=/app/data/gps.db"

COPY --from=build /app/publish .

# Se ejecuta con el usuario sin privilegios incluido en la imagen oficial de .NET.
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "ApiGps.Api.dll"]
