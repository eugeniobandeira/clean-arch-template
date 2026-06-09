# Clean Architecture Template

[![CI](https://github.com/eugeniobandeira/clean-arch-template/actions/workflows/ci.yml/badge.svg)](https://github.com/eugeniobandeira/clean-arch-template/actions/workflows/ci.yml)
![.NET](https://img.shields.io/badge/.NET-10-512BD4)

A .NET 10 project template for building Web APIs using Clean Architecture and Vertical Slice organization.

## Features

- **Clean Architecture** — strict layer separation: Domain, Application, Infrastructure, API
- **Vertical Slice** — features organized by operation (Create, GetById, GetAll, Update, Delete)
- **Minimal APIs** — ASP.NET Core endpoint-per-file pattern via `IEndpoint`
- **API Versioning** — URL path versioning (`/api/v1`) via `Asp.Versioning.Http`
- **Rate Limiting** — fixed window policy (configurable via `appsettings.json`)
- **Health Checks** — `/health` and `/alive` endpoints via .NET Aspire ServiceDefaults
- **Error Handling** — `ErrorOr` pattern with structured `ProblemDetails` responses
- **Validation** — FluentValidation with localized messages
- **Logging** — Serilog with structured output and correlation ID enrichment
- **Observability** — OpenTelemetry tracing and metrics via .NET Aspire
- **OpenAPI** — Scalar UI available in development

## Tech Stack

| Concern | Library |
|---|---|
| Framework | ASP.NET Core 10 Minimal APIs |
| Error Handling | ErrorOr 2.0.1 |
| Validation | FluentValidation 12.1.1 |
| Logging | Serilog 10.0.0 |
| API Versioning | Asp.Versioning.Http 8.1.0 |
| API Documentation | Scalar + Microsoft.AspNetCore.OpenApi |
| Orchestration | .NET Aspire 9.3.1 |
| Testing | xUnit, Moq, FluentAssertions, Bogus |
| Code Analysis | SonarAnalyzer.CSharp |

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [.NET Aspire workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling): `dotnet workload install aspire`

## Quick Start

**Install the template:**

```bash
dotnet new install .
```

**Create a new project:**

```bash
dotnet new clean-arch -n MyProject
```

## Project Structure

```
├── 01 - aspire/
│   ├── 01 - AppHost/          # Aspire orchestration host
│   └── 02 - ServiceDefaults/  # Shared observability and health check defaults
├── 02 - src/
│   ├── 01 - Api/              # Endpoints, middleware, extensions
│   ├── 02 - Application/      # Handlers, validators, mappers (vertical slices)
│   ├── 03 - Domain/           # Entities, interfaces, constants
│   ├── 04 - IoC/              # Dependency injection wiring
│   └── 05 - Infrastructure/   # Repository implementations
├── 03 - tests/
│   ├── 01 - Common/           # Shared test builders and fixtures
│   ├── 02 - Validators/       # FluentValidation unit tests
│   ├── 03 - Handlers/         # Handler unit tests
│   ├── 04 - Repositories/     # Repository tests
│   └── 05 - Integration/      # API integration tests
├── docs/                      # Architecture docs, ADRs, guides
├── k8s/                       # Kubernetes manifests (Kustomize)
├── Dockerfile
└── CleanArch.slnx
```

## Health Endpoints

| Route | Purpose |
|---|---|
| `GET /health` | All registered health checks |
| `GET /alive` | Liveness probe (checks tagged `live`) |

## Running with Docker

```bash
docker build -t cleanarch .
docker run -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Production cleanarch
```

## Running Locally

With Aspire orchestration (recommended):

```bash
dotnet run --project "01 - aspire/01 - AppHost/CleanArch.AppHost"
```

Or directly:

```bash
dotnet run --project "02 - src/01 - Api/CleanArch.Api"
```

## Contributing

See [docs/guides/](docs/guides/) for contributor workflows, testing guidelines, and publishing instructions.
