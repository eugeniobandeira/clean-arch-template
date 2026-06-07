# EugenioBandeira.CleanArchTemplate

.NET 10 template with Clean Architecture + Vertical Slice, Repository Pattern, feature handlers, CorrelationId, Serilog, and .NET Aspire.

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

## Implementing the repository

The template generates `ExampleRepository.cs` with stubs for all repository interfaces. Implement it using the persistence technology of your choice (EF Core, Dapper, etc.):

```csharp
public sealed class ExampleRepository :
    IAddRepository<ExampleEntity>,
    IGetByIdRepository<ExampleEntity>,
    IGetAllRepository<ExampleEntity, GetAllExampleRequest>,
    IUpdateRepository<ExampleEntity>,
    IDeleteRepository<ExampleEntity>
{
    // Implement the methods here
}
```

Then register it in `InfrastructureDependencyInjection.cs`:

```csharp
services.AddScoped<ExampleRepository>();
services.AddScoped<IAddRepository<ExampleEntity>>(sp => sp.GetRequiredService<ExampleRepository>());
services.AddScoped<IGetByIdRepository<ExampleEntity>>(sp => sp.GetRequiredService<ExampleRepository>());
services.AddScoped<IGetAllRepository<ExampleEntity, GetAllExampleRequest>>(sp => sp.GetRequiredService<ExampleRepository>());
services.AddScoped<IUpdateRepository<ExampleEntity>>(sp => sp.GetRequiredService<ExampleRepository>());
services.AddScoped<IDeleteRepository<ExampleEntity>>(sp => sp.GetRequiredService<ExampleRepository>());
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

## Post-generation setup

1. Update connection strings in `appsettings.json`
2. Replace `Jwt:SecretKey` with a secure key
3. Implement `ExampleRepository.cs` with your chosen persistence technology
4. Register the repository in `InfrastructureDependencyInjection.cs`

## Stack

| Layer | Technologies |
|---|---|
| API | ASP.NET Core Minimal APIs, JWT Bearer, Scalar (OpenAPI), Serilog |
| Application | FluentValidation, ErrorOr, generic `IHandler<TRequest, TResponse>` |
| Domain | Repository interfaces segregated by operation |
| Infrastructure | Repository stub, persistence-agnostic |
| Observability | .NET Aspire, OpenTelemetry, CorrelationId |
| Tests | xUnit, Moq, FluentAssertions, Bogus, WebApplicationFactory |

## Publish to NuGet

```bash
dotnet pack -c Release
dotnet nuget push bin/Release/EugenioBandeira.CleanArchTemplate.1.0.0.nupkg \
  --api-key $NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

## License

MIT © eugeniobandeira
