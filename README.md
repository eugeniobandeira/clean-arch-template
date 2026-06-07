# EugenioBandeira.CleanArchTemplate

.NET 10 template with Clean Architecture + Vertical Slice, Repository Pattern, feature handlers, JWT, CorrelationId, Serilog, and .NET Aspire.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- .NET Aspire workload:

```bash
dotnet workload install aspire
```

## Installation

```bash
dotnet new install EugenioBandeira.CleanArchTemplate
```

## Usage

```bash
dotnet new clean-arch -n MyApp
```

## Generated structure

```
MyApp/
├── 01 - aspire/
│   ├── 01 - AppHost/
│   │   └── MyApp.AppHost/              # .NET Aspire orchestration
│   └── 02 - ServiceDefaults/
│       └── MyApp.ServiceDefaults/      # OTEL, health checks, service discovery
├── 02 - src/
│   ├── 01 - Api/
│   │   └── MyApp.Api/                  # Minimal API, middlewares, endpoints per feature
│   ├── 02 - Application/
│   │   └── MyApp.Application/          # Handlers, validators, DTOs per feature (Vertical Slice)
│   ├── 03 - Domain/
│   │   └── MyApp.Domain/               # Entities, repository interfaces, constants
│   ├── 04 - IoC/
│   │   └── MyApp.IoC/                  # Dependency injection setup
│   └── 05 - Infrastructure/
│       └── MyApp.Infrastructure/       # Repository implementations
├── 03 - tests/
│   ├── 01 - Common/
│   │   └── MyApp.Tests.Common/         # Test builders with Bogus
│   ├── 02 - Validators/
│   │   └── MyApp.Tests.Validators/     # FluentValidation tests
│   ├── 03 - Handlers/
│   │   └── MyApp.Tests.Handlers/       # Handler unit tests
│   ├── 04 - Repositories/
│   │   └── MyApp.Tests.Repositories/   # Repository tests
│   └── 05 - Integration/
│       └── MyApp.Tests.Integration/    # Integration tests via WebApplicationFactory
├── docs/                               # Architecture docs, ADRs, feature plans
├── iac/                                # Infrastructure as Code (Terraform, Bicep, etc.)
└── k8s/                                # Kubernetes manifests (Kustomize)
    ├── base/
    └── overlays/
        ├── dev/
        └── prod/
```

## Post-generation setup

1. Update connection strings in `appsettings.json`
2. Replace `Jwt:SecretKey` with a secure key
3. Implement `ExampleRepository.cs` with your chosen persistence technology
4. Register the repository in `InfrastructureDependencyInjection.cs`
5. Register new handler types in `ApplicationDependencyInjection.cs` as you add features

> The `Example*` files throughout the project are working stubs that demonstrate the patterns. Use them as a reference, then replace them with your own features.

## Adding a new feature

The workflow for adding a feature (e.g. `Product`) mirrors the existing `Example` slice:

**1. Domain** — add entity and error codes:
```
Domain/Entities/ProductEntity.cs
Domain/Constants/ProductErrorCodes.cs
```

**2. Application** — add handlers, requests, validators:
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
    // Implement using EF Core, Dapper, DynamoDB, etc.
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

Endpoints are registered automatically via reflection — no manual wiring needed.

## Architecture

The template uses **Vertical Slice** with **Clean Architecture**. Each feature is self-contained within the Application layer.

### Handler pattern

All handlers implement the generic `IHandler<TRequest, TResponse>` interface:

```csharp
public interface IHandler<TRequest, TResponse>
{
    Task<ErrorOr<TResponse>> Handle(TRequest request, CancellationToken cancellationToken = default);
}
```

Each feature slice follows this structure:

```
Application/Features/Example/
├── ExampleResponse.cs
├── Mapper/
│   └── ExampleMapper.cs
└── Handlers/
    ├── Create/
    │   ├── CreateExampleHandler.cs        # IHandler<CreateExampleRequest, ExampleEntity>
    │   ├── Request/CreateExampleRequest.cs
    │   └── Validator/CreateExampleValidator.cs
    ├── GetById/
    │   └── GetByIdExampleHandler.cs       # IHandler<Guid, ExampleEntity>
    ├── GetAll/
    │   ├── GetAllExampleHandler.cs        # IHandler<GetAllExampleRequest, PagedResult<ExampleEntity>>
    │   └── Request/GetAllExampleRequest.cs
    ├── Update/
    │   ├── UpdateExampleHandler.cs        # IHandler<UpdateCommand<UpdateExampleRequest>, ExampleEntity>
    │   ├── Request/UpdateExampleRequest.cs
    │   └── Validator/UpdateExampleValidator.cs
    └── Delete/
        └── DeleteExampleHandler.cs        # IHandler<Guid, Deleted>
```

### Update command

Updates use the generic `UpdateCommand<T>` from `Common/Commands`:

```csharp
public sealed record UpdateCommand<T>(Guid Id, T Dto);
```

### Endpoints

Endpoints are registered automatically via reflection — each file implements `IEndpoint`:

```csharp
internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/examples", HandleAsync)
           .WithTags(Tags.EXAMPLE)
           .RequireAuthorization();
    }
}
```

## CorrelationId

Every request is tracked by a correlation ID that flows through logs, responses, and error payloads.

**How it works:**

1. The `CorrelationIdMiddleware` reads the `X-Correlation-Id` request header. If absent, it generates a new `Guid`.
2. The value is sanitized (alphanumeric + dashes, max 64 chars) to prevent header injection.
3. It is stored in `HttpContext.Items` and echoed back in the `X-Correlation-Id` response header.
4. It is pushed to Serilog's `LogContext` — every log line in that request automatically includes `{CorrelationId}`.
5. On unhandled exceptions, `GlobalExceptionHandler` includes it in the `ProblemDetails` extensions:

```json
{
  "status": 500,
  "title": "An unexpected error occurred.",
  "extensions": {
    "correlationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "traceId": "00-abc123..."
  }
}
```

**Propagating to downstream services:**

```csharp
httpClient.DefaultRequestHeaders.Add("X-Correlation-Id", correlationId);
```

## Shipping logs and telemetry

The template supports two complementary approaches.

### Option 1 — Serilog sink (logs only)

Install the sink for your platform and add it to the `Serilog` configuration in `appsettings.json`:

```bash
dotnet add package Serilog.Sinks.Datadog.Logs   # Datadog
dotnet add package Serilog.Sinks.Elasticsearch  # Elastic
dotnet add package Serilog.Sinks.Seq            # Seq
dotnet add package Serilog.Sinks.Splunk         # Splunk
```

```json
"Serilog": {
  "WriteTo": [
    { "Name": "Console" },
    {
      "Name": "<SinkName>",
      "Args": {
        // sink-specific arguments
      }
    }
  ]
}
```

### Option 2 — OpenTelemetry OTLP (logs + traces + metrics)

The template already includes `OpenTelemetry.Exporter.OpenTelemetryProtocol` via Aspire. Point the OTLP exporter to your collector or platform agent:

```bash
OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4318
```

Most observability platforms accept OTLP natively (Datadog, Grafana, Honeycomb, Elastic, Jaeger, etc.) either directly or via an OpenTelemetry Collector.

### Comparison

| | Serilog sink | OTLP |
|---|---|---|
| Covers | Logs only | Logs + Traces + Metrics |
| Extra packages | Yes (one per platform) | No (already included) |
| Platform-specific config | Yes | No — standard protocol |

For full observability (logs, traces, and metrics) with any platform, **OTLP is the recommended approach**.

## Stack

| Layer | Technologies |
|---|---|
| API | ASP.NET Core Minimal APIs, JWT Bearer, Scalar (OpenAPI), Serilog |
| Application | FluentValidation, ErrorOr, generic `IHandler<TRequest, TResponse>` |
| Domain | Repository interfaces segregated by operation |
| Infrastructure | Repository stub, persistence-agnostic |
| Observability | .NET Aspire, OpenTelemetry, CorrelationId |
| Tests | xUnit, Moq, FluentAssertions, Bogus, WebApplicationFactory |

## Contributing

1. Fork the repository and create a branch from `master`:
   ```bash
   git checkout -b feat/your-feature
   ```
2. Make your changes inside `templates/clean-arch/`
3. Verify the template builds cleanly:
   ```bash
   dotnet build templates/clean-arch/CleanArch.slnx -c Release
   ```
4. Commit using [Conventional Commits](https://www.conventionalcommits.org/) and open a PR targeting `master`
5. CI runs automatically on every push and PR — the build must pass before merging
6. Once merged, bump `PackageVersion` in `clean-arch-template.csproj` to trigger the publish pipeline

## License

MIT © eugeniobandeira
