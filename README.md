# EugenioBandeira.CleanArchTemplate

Template .NET 10 com Clean Architecture + Vertical Slice, Repository Pattern, EF Core, DynamoDB, JWT e CorrelationId.

## Instalação

```bash
dotnet new install EugenioBandeira.CleanArchTemplate
```

## Uso

```bash
dotnet new clean-arch -n MinhaApp
```

### Parâmetros opcionais

| Parâmetro | Padrão | Descrição |
|---|---|---|
| `--useEfCore` | `true` | Inclui persistência EF Core |
| `--useDynamoDB` | `false` | Inclui persistência DynamoDB |
| `--useAuth` | `true` | Inclui autenticação JWT |

```bash
# Exemplo com DynamoDB habilitado
dotnet new clean-arch -n MinhaApp --useDynamoDB true
```

## Estrutura gerada

```
MinhaApp/
├── src/
│   ├── MinhaApp.Api/               # Minimal API, Middlewares, Endpoints por feature
│   ├── MinhaApp.Application/       # Services, DTOs, Validators por feature (Vertical Slice)
│   ├── MinhaApp.Domain/            # Entities, AggregateRoot, Repository interfaces
│   └── MinhaApp.Infrastructure/    # EF Core, DynamoDB, implementações dos repositories
└── tests/
    ├── MinhaApp.UnitTests/          # xUnit + Moq + FluentAssertions
    └── MinhaApp.IntegrationTests/   # WebApplicationFactory
```

## Configuração pós-geração

1. Atualize a connection string em `appsettings.json`
2. Troque o `Jwt:SecretKey` por uma chave segura
3. Rode as migrations: `dotnet ef migrations add Initial -p src/MinhaApp.Infrastructure -s src/MinhaApp.Api`
4. Aplique: `dotnet ef database update -s src/MinhaApp.Api`

## EF Core vs DynamoDB

- **EF Core** (`SampleEfRepository`): para entidades relacionais
- **DynamoDB** (`SampleDynamoRepository`): para eventos, audit logs, sessões

Para trocar a implementação, edite `InfrastructureExtensions.cs`:

```csharp
// EF Core (padrão)
services.AddScoped<ISampleRepository, SampleEfRepository>();

// DynamoDB
services.AddScoped<ISampleRepository, SampleDynamoRepository>();
```

## Publicar no NuGet

```bash
dotnet pack -c Release
dotnet nuget push bin/Release/EugenioBandeira.CleanArchTemplate.1.0.0.nupkg \
  --api-key $NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

## Licença

MIT © eugeniobandeira
