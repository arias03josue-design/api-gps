# API GPS

API minimal en **.NET 10** para registrar y consultar las posiciones GPS que reportan los clientes durante sus rutas.
Usa Clean Architecture, CQRS, Riok.Mapperly, NSwag y SQLite, y se despliega con Docker con
almacenamiento persistente.

## Endpoints

| Método | Ruta           | Autenticación    | Descripción                                                          |
|--------|----------------|------------------|----------------------------------------------------------------------|
| PUT    | `/coordinate`  | Header `ApiKey`  | Registra una posición del cliente en una ruta. Responde `200 OK` sin cuerpo. |
| GET    | `/coordinate?client={client}&routeId={routeId}` | Header `ApiKey` | Lista las posiciones de una ruta, de la más antigua a la más reciente. `client` y `routeId` son obligatorios. |
| PUT    | `/route`       | Header `ApiKey`  | Registra el estado (`Activo` o `Completado`) de la ruta de un cliente. Responde `200 OK` sin cuerpo. |
| GET    | `/health`      | —                | Estado de la API y de la base de datos.                              |
| GET    | `/swagger`     | —                | Swagger UI generado con NSwag (`/swagger/v1/swagger.json`).          |

**PUT /coordinate**

```json
{
  "client": "device-01",
  "routeId": "ruta-001",
  "coordinate": "9.9281,-84.0907",
  "createdAt": "2026-11-18T15:30:00Z"
}
```

**GET /coordinate?client=device-01&routeId=ruta-001**

```json
[
  { "client": "device-01", "routeId": "ruta-001", "coordinate": "9.9281,-84.0907", "createdAt": "2026-11-18T15:30:00+00:00" },
  { "client": "device-01", "routeId": "ruta-001", "coordinate": "9.9290,-84.0900", "createdAt": "2026-11-18T15:31:00+00:00" }
]
```

**PUT /route**

```json
{
  "routeId": "ruta-001",
  "client": "device-01",
  "status": "Activo"
}
```

### Reglas de negocio

- **Historial por ruta**: cada PUT agrega la posición al historial de la ruta indicada en `routeId`.
  Es un texto obligatorio de hasta 100 caracteres (GUID, número o código, según lo genere el cliente).
- **Idempotencia del PUT**: la combinación `client` + `routeId` + `createdAt` es única. Reenviar la
  misma lectura (por ejemplo, un reintento) **actualiza** esa posición en lugar de duplicarla. Es un
  upsert atómico de SQLite (`INSERT ... ON CONFLICT DO UPDATE`), seguro ante peticiones concurrentes.
- **GET por ruta**: exige `client` y `routeId` como parámetros de consulta y devuelve solo las
  posiciones de esa ruta, ordenadas por `createdAt` de la más antigua a la más reciente (las del mismo
  instante, por orden de llegada). Los valores se comparan de forma exacta: se ignoran los espacios
  alrededor, pero se distinguen mayúsculas y minúsculas. Si la ruta no tiene posiciones, devuelve `[]`.
- **Estado de la ruta (`PUT /route`)**: hay una sola ruta por `client` + `routeId`. Reenviarla
  **actualiza** su estado (por ejemplo, de `Activo` a `Completado`) con el mismo upsert atómico.
  `status` es obligatorio y solo admite `Activo` o `Completado`, como texto (sin distinguir
  mayúsculas; no se aceptan números). Cualquier otro valor responde `400`. El `routeId` es el mismo
  que se envía en las posiciones, pero la API no exige que la ruta exista antes de recibir posiciones.
- **Coordenada**: formato `latitud,longitud` en grados decimales con punto como separador decimal
  (latitud entre -90 y 90, longitud entre -180 y 180). Se eliminan los espacios alrededor de cada valor.
- **Fechas**: ISO 8601. Se guardan y se devuelven en UTC, por lo que `09:30:00-06:00` y
  `15:30:00Z` son el mismo instante (y el mismo registro). Si la fecha no trae zona horaria se
  asume la del servidor (UTC dentro de Docker).
- **Errores**: siempre en formato ProblemDetails (RFC 9457).
  - `400`: validación, con el detalle por campo en `errors`, o un JSON mal formado.
  - `401`: falta el header `ApiKey` o su valor no es válido.
  - `500`: error inesperado del servidor.

## Arquitectura

```
src/
├── Domain          Entidades, objetos de valor y reglas de negocio. Sin dependencias.
├── Application     Casos de uso (CQRS), validaciones, mapeos (Mapperly) e interfaces de persistencia.
├── Infrastructure  EF Core + SQLite: DbContext, repositorios y migraciones.
└── Api             Minimal API, autenticación por ApiKey, NSwag y manejo de errores.
```

Las dependencias apuntan hacia el dominio: `Api → Application → Domain` e `Infrastructure → Application`.
`Api` referencia a `Infrastructure` solo para registrar sus servicios (raíz de composición).

### CQRS

| Lado     | Flujo                                                                                                                                                                   |
|----------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Comando  | `UpsertCoordinateCommand` → `ValidationCommandHandlerDecorator` → `UpsertCoordinateCommandHandler` → `ClientCoordinate.Create` (dominio) → `ICoordinateRepository` (upsert) |
| Comando  | `UpsertRouteCommand` → `ValidationCommandHandlerDecorator` → `UpsertRouteCommandHandler` → `ClientRoute.Create` (dominio) → `IRouteRepository` (upsert) |
| Consulta | `GetCoordinatesQuery` → `ValidationQueryHandlerDecorator` → `GetCoordinatesQueryHandler` → `ICoordinateReadRepository` (filtro por cliente y ruta, proyección `AsNoTracking` directa al DTO) |

La implementación de CQRS es propia y ligera (`ICommand`, `IQuery<T>` y sus handlers). No usa
MediatR, que desde la versión 13 requiere licencia comercial. Cada endpoint recibe su handler por
inyección de dependencias, y todo comando o consulta pasa por un decorador de validación antes de ejecutarse.

### Mapperly y validaciones

- `Api/Mapping/ContractMapper`: request → comando y DTO → response.
- `Application/Coordinates/CoordinateMapper`: entidad → DTO, incluida la proyección `IQueryable`
  que EF Core traduce a SQL.
- **Validación de mapeos en compilación**: los mappers usan `RequiredMappingStrategy.Both` y
  `.editorconfig` eleva los diagnósticos `RMG012` y `RMG020` a error. Si una propiedad de un contrato
  queda sin mapear, el proyecto no compila.
- **Validación de datos**: Mapperly genera código de mapeo, no valida datos en tiempo de ejecución.
  Los datos de entrada se validan en `UpsertCoordinateCommandValidator` y `GetCoordinatesQueryValidator`,
  que reportan todos los errores a la vez, y en las invariantes del dominio (`ClientCoordinate`, `GeoCoordinate`).

## Configuración

| Clave                        | Variable de entorno           | Valor por defecto         | Descripción                                   |
|------------------------------|-------------------------------|---------------------------|-----------------------------------------------|
| `ApiKey:Value`               | `ApiKey__Value`               | *(obligatoria)*           | Clave que deben enviar los clientes. La API no arranca si no se define. |
| `ApiKey:HeaderName`          | `ApiKey__HeaderName`          | `ApiKey`                  | Header en el que se envía la clave.           |
| `ConnectionStrings:Database` | `ConnectionStrings__Database` | `Data Source=data/gps.db` | Archivo SQLite (en Docker: `/app/data/gps.db`). |
| `Swagger:Enabled`            | `Swagger__Enabled`            | `true`                    | Publica Swagger UI y el documento OpenAPI.    |

## Ejecución local

Requisitos: SDK de .NET 10.

```bash
dotnet run --project src/Api
```

Swagger UI queda en http://localhost:5242/swagger. En desarrollo la ApiKey está en
`src/Api/appsettings.Development.json`. La base de datos se crea en `src/Api/data/`.

## Docker

Requisitos: Docker con Compose. En Windows, Docker Desktop necesita WSL 2.

```bash
cp .env.example .env
```

Edite `.env`, defina `GPS_API_KEY` y luego:

```bash
docker compose up -d --build
```

La API queda en http://localhost:8080. Para cambiar el puerto, use `GPS_API_PORT` en `.env`.

### Persistencia de datos

La base de datos SQLite vive en el volumen con nombre **`api-gps-data`**, montado en `/app/data`.
El volumen es independiente del contenedor:

| Acción                                                              | ¿Se conservan los datos? |
|---------------------------------------------------------------------|--------------------------|
| `docker compose restart` / `stop` / `down`                          | Sí                       |
| Recrear o eliminar el contenedor (`up -d --build`, `docker rm -f api-gps`) | Sí                 |
| Actualizar la imagen o la aplicación                                | Sí (las migraciones nuevas se aplican al arrancar) |
| `docker compose down -v` o `docker volume rm api-gps-data`          | **No**: se elimina el volumen |

Respaldo del volumen (detenga la API para obtener una copia consistente):

```bash
docker compose stop
```

```bash
docker run --rm -v api-gps-data:/data -v "$(pwd)":/backup alpine tar czf /backup/gps-data.tgz -C /data .
```

```bash
docker compose start
```

Notas de despliegue:

- Ejecute **una sola instancia**. SQLite no admite varias réplicas escribiendo sobre el mismo archivo.
- La API escucha HTTP en el puerto 8080. Como la ApiKey viaja en un header, publíquela detrás de
  un proxy inverso con HTTPS (Nginx, Traefik, etc.).
- El contenedor se ejecuta con el usuario sin privilegios `app` (UID 1654). Si en lugar del volumen
  con nombre monta una carpeta del host, esa carpeta debe tener permisos de escritura para ese UID.

## Migraciones de base de datos

Las migraciones se aplican automáticamente al iniciar la API. Para crear una nueva tras cambiar el modelo:

```bash
dotnet tool restore
```

```bash
dotnet ef migrations add NombreDeLaMigracion --project src/Infrastructure --startup-project src/Api --output-dir Persistence/Migrations
```

## Ejemplos con curl

```bash
curl -X PUT http://localhost:8080/coordinate -H "ApiKey: <su-clave>" -H "Content-Type: application/json" -d '{"client":"device-01","routeId":"ruta-001","coordinate":"9.9281,-84.0907","createdAt":"2026-11-18T15:30:00Z"}'
```

```bash
curl "http://localhost:8080/coordinate?client=device-01&routeId=ruta-001" -H "ApiKey: <su-clave>"
```

```bash
curl -X PUT http://localhost:8080/route -H "ApiKey: <su-clave>" -H "Content-Type: application/json" -d '{"routeId":"ruta-001","client":"device-01","status":"Completado"}'
```
