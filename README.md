# EugenioBandeira.CleanArchTemplate

[![CI](https://github.com/eugeniobandeira/clean-arch-template/actions/workflows/ci.yml/badge.svg)](https://github.com/eugeniobandeira/clean-arch-template/actions/workflows/ci.yml)
![.NET](https://img.shields.io/badge/.NET-10-512BD4)
[![NuGet](https://img.shields.io/nuget/v/EugenioBandeira.CleanArchTemplate)](https://www.nuget.org/packages/EugenioBandeira.CleanArchTemplate)

.NET 10 `dotnet new` template for building production-ready Web APIs with Clean Architecture, Vertical Slice, and .NET Aspire.

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
dotnet new clean-arch -n MyProject
```

## What You Get

| Concern | What's included |
|---|---|
| Architecture | Clean Architecture + Vertical Slice, `IHandler<TRequest, TResponse>` pattern |
| Error Handling | `ErrorOr` result type, `GlobalExceptionHandler`, RFC 7807 `ProblemDetails` |
| Validation | FluentValidation wired into handlers |
| Logging | Serilog with structured output, correlation ID enrichment |
| Observability | OpenTelemetry (traces, metrics, logs) via .NET Aspire ServiceDefaults |
| API | Minimal APIs, URL versioning (`/api/v1`), Scalar (OpenAPI), rate limiting, CORS |
| Health Checks | `/health`, `/alive`, `/api/v1/health` |
| Repository Pattern | Segregated interfaces per operation (`IAddRepository`, `IGetByIdRepository`, etc.) |
| Tests | xUnit, Moq, FluentAssertions, Bogus — unit + integration test projects |
| Infrastructure | Dockerfile, Kubernetes manifests (Kustomize), IaC folder |

## Generated Structure

```
MyProject/
├── 01 - aspire/
│   ├── 01 - AppHost/          # Aspire orchestration host
│   └── 02 - ServiceDefaults/  # Shared OTEL, health checks, service discovery
├── 02 - src/
│   ├── 01 - Api/              # Endpoints, middlewares, extensions
│   ├── 02 - Application/      # Handlers, validators, mappers (vertical slices)
│   ├── 03 - Domain/           # Entities, interfaces, constants
│   ├── 04 - IoC/              # Dependency injection wiring
│   └── 05 - Infrastructure/   # Repository implementations
├── 03 - tests/
│   ├── 01 - Common/           # Shared test builders
│   ├── 02 - Validators/       # FluentValidation tests
│   ├── 03 - Handlers/         # Handler unit tests
│   ├── 04 - Repositories/     # Repository tests
│   └── 05 - Integration/      # API integration tests
├── docs/                      # Architecture docs and ADRs
├── iac/                       # Infrastructure as Code
├── k8s/                       # Kubernetes manifests
└── Dockerfile
```

## Post-Generation Setup

1. Update `ConnectionStrings.DefaultConnection` in `appsettings.json`
2. Implement `ExampleRepository.cs` with your chosen persistence technology
3. Register the repository in `InfrastructureDependencyInjection.cs`
4. Update `Cors.AllowedOrigins` with your frontend URL
5. Update `OpenApi` contact info in `appsettings.json`
6. Set `OTEL_EXPORTER_OTLP_ENDPOINT` to enable observability export (optional)
7. Replace the `Example*` stubs with your own features

> Full documentation, architecture details, middleware descriptions, and the step-by-step guide for adding new features are in the generated project's `README.md`.

## Contributing

1. Fork and branch from `master`:

```bash
git checkout -b feat/your-feature
```

2. Make changes inside `templates/clean-arch/`
3. Verify the template builds:

```bash
dotnet build templates/clean-arch/CleanArch.slnx -c Release
```

4. Commit using [Conventional Commits](https://www.conventionalcommits.org/) and open a PR targeting `master`
5. After merging, bump `PackageVersion` in `clean-arch-template.csproj` to publish a new NuGet release

## License

MIT © eugeniobandeira
