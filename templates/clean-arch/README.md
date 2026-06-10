# Clean Architecture Template

[![CI](https://github.com/eugeniobandeira/clean-arch-template/actions/workflows/ci.yml/badge.svg)](https://github.com/eugeniobandeira/clean-arch-template/actions/workflows/ci.yml)
![.NET](https://img.shields.io/badge/.NET-10-512BD4)

A .NET 10 project template for building production-ready Web APIs using Clean Architecture and Vertical Slice organization.

## Tech Stack

| Concern | Library / Technology |
|---|---|
| Framework | ASP.NET Core 10 Minimal APIs |
| Error Handling | ErrorOr 2.0.1 |
| Validation | FluentValidation 12.1.1 |
| Logging | Serilog 10.0.0 |
| API Versioning | Asp.Versioning.Http 8.1.0 |
| API Documentation | Scalar + Microsoft.AspNetCore.OpenApi |
| Orchestration | .NET Aspire 9.3.1 |
| Observability | OpenTelemetry (traces, metrics, logs) |
| Testing | xUnit, Moq, FluentAssertions, Bogus |
| Code Analysis | SonarAnalyzer.CSharp |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- .NET Aspire workload:

```bash
dotnet workload install aspire
```

## Quick Start

```bash
dotnet new install EugenioBandeira.CleanArchTemplate
dotnet new clean-arch -n MyProject
```

## Project Structure

```
MyProject/
├── 01 - aspire/
│   ├── 01 - AppHost/
│   │   └── MyProject.AppHost/              # .NET Aspire orchestration
│   └── 02 - ServiceDefaults/
│       └── MyProject.ServiceDefaults/      # OpenTelemetry, health checks, service discovery
├── 02 - src/
│   ├── 01 - Api/
│   │   └── MyProject.Api/                  # Endpoints, middlewares, extensions
│   ├── 02 - Application/
│   │   └── MyProject.Application/          # Handlers, validators, DTOs (Vertical Slice)
│   ├── 03 - Domain/
│   │   └── MyProject.Domain/               # Entities, repository interfaces, constants
│   ├── 04 - IoC/
│   │   └── MyProject.IoC/                  # Dependency injection wiring
│   └── 05 - Infrastructure/
│       └── MyProject.Infrastructure/       # Repository implementations
├── 03 - tests/
│   ├── 01 - Common/                        # Shared builders (Bogus)
│   ├── 02 - Validators/                    # FluentValidation unit tests
│   ├── 03 - Handlers/                      # Handler unit tests
│   ├── 04 - Repositories/                  # Repository tests
│   └── 05 - Integration/                   # API integration tests (WebApplicationFactory)
├── docs/                                   # Architecture docs, ADRs, feature specs
├── iac/                                    # Infrastructure as Code (Terraform, Bicep, etc.)
├── k8s/                                    # Kubernetes manifests (Kustomize)
│   ├── base/
│   └── overlays/
│       ├── dev/
│       └── prod/
├── Dockerfile
└── CleanArch.slnx
```

## Architecture

The template combines **Clean Architecture** with **Vertical Slice** organization.

### Layer responsibilities

| Layer | Responsibility | Allowed dependencies |
|---|---|---|
| Domain | Entities, repository interfaces, error codes | None |
| Application | Handlers, validators, DTOs, mappers | Domain |
| Infrastructure | Repository implementations | Domain |
| IoC | DI registration | All layers |
| Api | Endpoints, middlewares, HTTP mapping | Application, Domain |

### Handler pattern

Every use case implements `IHandler<TRequest, TResponse>`, returning `ErrorOr<T>` instead of throwing exceptions:

```csharp
public interface IHandler<TRequest, TResponse>
{
    Task<ErrorOr<TResponse>> Handle(TRequest request, CancellationToken cancellationToken = default);
}
```

### Repository interfaces

Repositories are segregated by operation — implement only what each feature needs:

```csharp
IAddRepository<TEntity>
IGetByIdRepository<TEntity>
IGetAllRepository<TEntity, TRequest>
IUpdateRepository<TEntity>
IDeleteRepository<TEntity>
```

### Vertical Slice layout

Each feature is fully self-contained inside `Application/Features/{Feature}/`:

```
Application/Features/Example/
├── ExampleResponse.cs
├── Mapper/ExampleMapper.cs
└── Handlers/
    ├── Create/
    │   ├── CreateExampleHandler.cs
    │   ├── Request/CreateExampleRequest.cs
    │   └── Validator/CreateExampleValidator.cs
    ├── GetById/GetByIdExampleHandler.cs
    ├── GetAll/
    │   ├── GetAllExampleHandler.cs
    │   └── Request/GetAllExampleRequest.cs
    ├── Update/
    │   ├── UpdateExampleHandler.cs
    │   ├── Request/UpdateExampleRequest.cs
    │   └── Validator/UpdateExampleValidator.cs
    └── Delete/DeleteExampleHandler.cs
```

### Update command

Updates use the generic `UpdateCommand<T>` wrapper to carry both the resource ID and the payload:

```csharp
public sealed record UpdateCommand<T>(Guid Id, T Dto);
```

### Endpoints

Each endpoint is a single file implementing `IEndpoint` and is auto-registered via reflection — no manual wiring:

```csharp
internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/examples", HandleAsync)
           .WithName("CreateExample")
           .WithTags(Tags.EXAMPLE);
    }
}
```

All endpoints are mounted under `/api/v1` with rate limiting applied automatically.

## Middlewares

### CorrelationIdMiddleware

Tracks every request end-to-end across logs, responses, and error payloads.

- Reads `X-Correlation-Id` from the request header; generates a new `Guid` if absent.
- Sanitizes the value (alphanumeric + dashes, max 64 chars) to prevent header injection.
- Stores the value in `HttpContext.Items` and echoes it in the `X-Correlation-Id` response header.
- Pushes it to Serilog's `LogContext` — every log line in that request automatically includes `{CorrelationId}`.

### GlobalExceptionHandler

Catches all unhandled exceptions and returns a structured `ProblemDetails` response (RFC 7807):

```json
{
  "status": 500,
  "title": "An unexpected error occurred.",
  "instance": "/api/v1/examples",
  "extensions": {
    "correlationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "traceId": "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01",
    "exceptionType": "System.InvalidOperationException",
    "exceptionMessage": "Sequence contains no elements."
  }
}
```

`OperationCanceledException` triggered by client disconnection returns HTTP 499 and is logged as a warning, not an error.

### Rate Limiting

Fixed window policy applied globally to all versioned endpoints. Configurable via `appsettings.json`:

```json
"RateLimit": {
  "PermitLimit": 100,
  "WindowSeconds": 60,
  "QueueLimit": 0
}
```

### CORS

Configured via `appsettings.json`. Update the allowed origins before going to production:

```json
"Cors": {
  "AllowedOrigins": [ "https://your-frontend.com" ]
}
```

## Health Endpoints

| Route | Purpose |
|---|---|
| `GET /health` | All registered health checks |
| `GET /alive` | Liveness probe (checks tagged `live`) |
| `GET /api/v1/health` | Application-level health check (versioned, documented in Swagger) |

## Error Handling

Business logic never throws — it returns `ErrorOr<T>`. Endpoints map the result to HTTP responses:

```csharp
ErrorOr<ExampleEntity> result = await handler.Handle(request, cancellationToken);

return result.Match(
    entity => Results.Ok(entity.ToResponse()),
    errors => errors.ToProblem(httpContext));
```

Error types are mapped to HTTP status codes automatically:

| ErrorOr type | HTTP status |
|---|---|
| `Error.Validation` | 422 Unprocessable Entity |
| `Error.NotFound` | 404 Not Found |
| `Error.Conflict` | 409 Conflict |
| `Error.Unauthorized` | 401 Unauthorized |
| Other | 500 Internal Server Error |

## Observability

The template ships with OpenTelemetry pre-configured for traces, metrics, and logs via `.NET Aspire ServiceDefaults`.

Set the following environment variables to enable export to any OTLP-compatible collector:

| Variable | Description | Default |
|---|---|---|
| `OTEL_EXPORTER_OTLP_ENDPOINT` | Collector URL | _(empty — export disabled)_ |
| `OTEL_EXPORTER_OTLP_PROTOCOL` | `http/protobuf` or `grpc` | `http/protobuf` |
| `OTEL_EXPORTER_OTLP_HEADERS` | Auth headers (e.g. API key) | _(empty)_ |
| `OTEL_SERVICE_NAME` | Service name in traces/metrics | `CleanArch.Api` |
| `OTEL_RESOURCE_ATTRIBUTES` | Additional resource metadata | `deployment.environment=production` |

Compatible platforms: Grafana Cloud, Datadog, New Relic, Honeycomb, Elastic, Jaeger, OpenTelemetry Collector.

### Serilog sinks (logs only)

If you prefer sending logs via a Serilog sink instead of OTLP:

```bash
dotnet add package Serilog.Sinks.Seq
dotnet add package Serilog.Sinks.Datadog.Logs
dotnet add package Serilog.Sinks.Elasticsearch
```

Configure in `appsettings.json` under `Serilog.WriteTo`.

## Post-Generation Setup

After running `dotnet new clean-arch -n MyProject`, complete the following steps:

### 1. Replace the connection string

In `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "your-real-connection-string"
}
```

### 2. Implement the repository

Open `Infrastructure/Repositories/ExampleRepository.cs` and implement the methods using your chosen persistence technology (EF Core, Dapper, MongoDB, etc.):

```csharp
public sealed class ExampleRepository :
    IAddRepository<ExampleEntity>,
    IGetByIdRepository<ExampleEntity>,
    IGetAllRepository<ExampleEntity, GetAllExampleRequest>,
    IUpdateRepository<ExampleEntity>,
    IDeleteRepository<ExampleEntity>
{
    // Your implementation here
}
```

### 3. Update CORS origins

In `appsettings.json`, replace the placeholder with your frontend URL:

```json
"Cors": {
  "AllowedOrigins": [ "https://your-frontend.com" ]
}
```

### 4. Update OpenAPI contact info

In `appsettings.json`:

```json
"OpenApi": {
  "ContactName": "your-name",
  "ContactUrl": "https://github.com/your-handle"
}
```

### 5. Configure observability (optional)

Set `OTEL_EXPORTER_OTLP_ENDPOINT` to point to your collector. Leave it empty to disable export during local development.

### 6. Replace the Example stubs

The `Example*` files throughout the project are working stubs that demonstrate all patterns end-to-end. Use them as a reference, then replace them with your own features.

## Adding a New Feature

The workflow for adding a feature (e.g. `Product`) mirrors the existing `Example` slice:

**1. Domain** — add entity and error codes:

```
Domain/Entities/ProductEntity.cs
Domain/Constants/ProductErrorCodes.cs
```

**2. Application** — add handlers, requests, validators, mapper:

```
Application/Features/Products/
├── ProductResponse.cs
├── Mapper/ProductMapper.cs
└── Handlers/
    ├── Create/
    │   ├── CreateProductHandler.cs
    │   ├── Request/CreateProductRequest.cs
    │   └── Validator/CreateProductValidator.cs
    ├── GetById/GetByIdProductHandler.cs
    ├── GetAll/
    │   ├── GetAllProductHandler.cs
    │   └── Request/GetAllProductRequest.cs
    ├── Update/
    │   ├── UpdateProductHandler.cs
    │   ├── Request/UpdateProductRequest.cs
    │   └── Validator/UpdateProductValidator.cs
    └── Delete/DeleteProductHandler.cs
```

**3. Infrastructure** — implement the repository:

```csharp
public sealed class ProductRepository :
    IAddRepository<ProductEntity>,
    IGetByIdRepository<ProductEntity>,
    IGetAllRepository<ProductEntity, GetAllProductRequest>,
    IUpdateRepository<ProductEntity>,
    IDeleteRepository<ProductEntity>
{
    // EF Core, Dapper, etc.
}
```

**4. IoC** — register handlers and repository:

In `ApplicationDependencyInjection.cs`:

```csharp
services.AddScoped<IHandler<CreateProductRequest, ProductEntity>, CreateProductHandler>();
services.AddScoped<IHandler<Guid, ProductEntity>, GetByIdProductHandler>();
services.AddScoped<IHandler<GetAllProductRequest, PagedResult<ProductEntity>>, GetAllProductHandler>();
services.AddScoped<IHandler<UpdateCommand<UpdateProductRequest>, ProductEntity>, UpdateProductHandler>();
services.AddScoped<IHandler<Guid, Deleted>, DeleteProductHandler>();
```

In `InfrastructureDependencyInjection.cs`:

```csharp
services.AddScoped<ProductRepository>();
services.AddScoped<IAddRepository<ProductEntity>>(sp => sp.GetRequiredService<ProductRepository>());
services.AddScoped<IGetByIdRepository<ProductEntity>>(sp => sp.GetRequiredService<ProductRepository>());
services.AddScoped<IGetAllRepository<ProductEntity, GetAllProductRequest>>(sp => sp.GetRequiredService<ProductRepository>());
services.AddScoped<IUpdateRepository<ProductEntity>>(sp => sp.GetRequiredService<ProductRepository>());
services.AddScoped<IDeleteRepository<ProductEntity>>(sp => sp.GetRequiredService<ProductRepository>());
```

**5. Api** — add one file per endpoint:

```
Api/Endpoints/Products/
├── Create.cs
├── GetById.cs
├── GetAll.cs
├── Update.cs
└── Delete.cs
```

Endpoints are registered automatically via reflection — no additional wiring needed.

## Running Locally

With Aspire orchestration (recommended):

```bash
dotnet run --project "01 - aspire/01 - AppHost/CleanArch.AppHost"
```

Or directly:

```bash
dotnet run --project "02 - src/01 - Api/CleanArch.Api"
```

## Running with Docker

```bash
docker build -t myproject .
docker run -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Production myproject
```

## Running on Kubernetes

```bash
kubectl apply -k k8s/overlays/dev
kubectl apply -k k8s/overlays/prod
```

## Contributing

See [docs/](docs/) for architecture docs, ADRs, and contributor guides.

## License

MIT © eugeniobandeira
