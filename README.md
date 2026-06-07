# EugenioBandeira.CleanArchTemplate

.NET 10 template with Clean Architecture + Vertical Slice, Repository Pattern, feature handlers, JWT, CorrelationId, Serilog, and .NET Aspire.

## Installation

```bash
dotnet new install EugenioBandeira.CleanArchTemplate
```

## Usage

```bash
dotnet new clean-arch -n MyApp
```

### Optional parameters

| Parameter | Default | Description |
|---|---|---|
| `--useEfCore` | `true` | Prepares the project for EF Core persistence |
| `--useDynamoDB` | `false` | Prepares the project for DynamoDB (AWS) persistence |

```bash
# Example with DynamoDB
dotnet new clean-arch -n MyApp --useDynamoDB true
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
│   │   └── MyApp.Domain/               # Entities, repository interfaces, filters, constants
│   ├── 04 - IoC/
│   │   └── MyApp.IoC/                  # Dependency injection setup
│   └── 05 - Infrastructure/
│       └── MyApp.Infrastructure/       # Repository implementations
└── 03 - tests/
    ├── 01 - Common/
    │   └── MyApp.Tests.Common/         # Test builders with Bogus
    ├── 02 - Validators/
    │   └── MyApp.Tests.Validators/     # FluentValidation tests
    ├── 03 - Handlers/
    │   └── MyApp.Tests.Handlers/       # Handler unit tests
    ├── 04 - Repositories/
    │   └── MyApp.Tests.Repositories/   # Repository tests
    └── 05 - Integration/
        └── MyApp.Tests.Integration/    # Integration tests via WebApplicationFactory
```

## Architecture

The template uses **Vertical Slice** with **Clean Architecture**. Each feature is self-contained within the Application layer:

```
Application/Features/Example/
├── ExampleResponse.cs
├── Mapper/ExampleMapper.cs
└── Handlers/
    ├── Create/  → ICreateExampleHandler, CreateExampleHandler, CreateExampleRequest, CreateExampleValidator
    ├── GetById/ → IGetByIdExampleHandler, GetByIdExampleHandler
    ├── GetAll/  → IGetAllExampleHandler, GetAllExampleHandler
    ├── Update/  → IUpdateExampleHandler, UpdateExampleHandler, UpdateExampleRequest, UpdateExampleValidator
    └── Delete/  → IDeleteExampleHandler, DeleteExampleHandler
```

Endpoints (Api layer) are registered automatically via reflection — each file implements `IEndpoint`:

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

The template generates `ExampleRepository.cs` with stubs for all repository interfaces. Implement it using the persistence technology of your choice (EF Core, Dapper, DynamoDB, etc.):

```csharp
public sealed class ExampleRepository :
    IAddRepository<ExampleEntity>,
    IGetByIdRepository<ExampleEntity>,
    IGetAllRepository<ExampleEntity, ExampleFilter>,
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
services.AddScoped<IGetAllRepository<ExampleEntity, ExampleFilter>>(sp => sp.GetRequiredService<ExampleRepository>());
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

**Reading the correlation ID inside a handler:**

Inject `IHttpContextAccessor` and read from `HttpContext.Items`:

```csharp
public sealed class MyHandler(IHttpContextAccessor httpContextAccessor) : IMyHandler
{
    public Task Handle(...)
    {
        string? correlationId = httpContextAccessor.HttpContext?
            .Items[CorrelationIdConstants.Key]?.ToString();
    }
}
```

**Propagating to downstream services:**

Forward the header when calling external APIs:

```csharp
httpClient.DefaultRequestHeaders.Add("X-Correlation-Id", correlationId);
```

## Shipping logs and telemetry

The template supports two complementary approaches. Use one or both depending on your platform.

### Option 1 — Serilog sink (logs only)

Install the sink for your platform and add it to the `Serilog` configuration in `appsettings.json`:

```bash
# Examples
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
| Application | FluentValidation, ErrorOr |
| Domain | Repository interfaces segregated by operation |
| Infrastructure | Repository stub ready for EF Core or DynamoDB |
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
