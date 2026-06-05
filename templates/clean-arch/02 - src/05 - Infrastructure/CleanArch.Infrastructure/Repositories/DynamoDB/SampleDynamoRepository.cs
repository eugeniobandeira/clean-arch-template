using Amazon.DynamoDBv2.DataModel;
using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Filters;
using CleanArch.Domain.Interfaces.Common;
using CleanArch.Infrastructure.Persistence.DynamoDB;

namespace CleanArch.Infrastructure.Repositories.DynamoDB;

/// <summary>
/// Implementação alternativa do repositório usando DynamoDB.
/// Troque o registro no DI para usar esta implementação.
/// ATENÇÃO: DynamoDB não suporta transações compartilhadas com EF Core.
/// Ao usar esta implementação, o IUnitOfWork.CommitAsync() não tem efeito —
/// cada operação de escrita é auto-committed diretamente no SDK.
/// </summary>
public sealed class SampleDynamoRepository(DynamoDbContext dynamoContext) :
    IAddRepository<SampleEntity>,
    IGetByIdRepository<SampleEntity>,
    IUpdateRepository<SampleEntity>,
    IGetAllRepository<SampleEntity, SampleFilter>,
    IDeleteRepository<SampleEntity>
{
    public async Task<SampleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        SampleDynamoDocument? item = await dynamoContext.Context.LoadAsync<SampleDynamoDocument>(
            id.ToString(), cancellationToken);

        return item is null ? null : item.ToDomain();
    }

    public async Task<PagedResult<SampleEntity>> GetAllAsync(SampleFilter? filter = null, CancellationToken cancellationToken = default)
    {
        List<ScanCondition> conditions = new();
        IAsyncSearch<SampleDynamoDocument> search = dynamoContext.Context.ScanAsync<SampleDynamoDocument>(conditions);
        List<SampleDynamoDocument> results = await search.GetRemainingAsync(cancellationToken);

        IEnumerable<SampleDynamoDocument> filtered = filter?.IsActive.HasValue is true
            ? results.Where(x => x.IsActive == filter.IsActive.Value)
            : results;

        List<SampleDynamoDocument> filteredList = [.. filtered];
        int total = filteredList.Count;

        int page = filter?.Page ?? 1;
        int pageSize = filter?.PageSize ?? 10;

        IReadOnlyCollection<SampleEntity> items = [.. filteredList
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToDomain())];

        return new PagedResult<SampleEntity>(items, total);
    }

    public async Task AddAsync(SampleEntity entity, CancellationToken cancellationToken = default)
    {
        SampleDynamoDocument document = SampleDynamoDocument.FromDomain(entity);
        await dynamoContext.Context.SaveAsync(document, cancellationToken);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("SonarAnalyzer", "S4144", Justification = "DynamoDB SaveAsync is an upsert — functionally identical to AddAsync by design.")]
    public async Task UpdateAsync(SampleEntity entity, CancellationToken cancellationToken = default)
    {
        // DynamoDB SaveAsync is an upsert — functionally identical to AddAsync.
        // Kept as a separate method to satisfy the IUpdateRepository<T> contract.
        SampleDynamoDocument document = SampleDynamoDocument.FromDomain(entity);
        await dynamoContext.Context.SaveAsync(document, cancellationToken);
    }

    public async Task DeleteAsync(SampleEntity entity, CancellationToken cancellationToken = default)
        => await dynamoContext.Context.DeleteAsync<SampleDynamoDocument>(entity.Id.ToString(), cancellationToken);
}

[DynamoDBTable("Samples")]
public sealed class SampleDynamoDocument
{
    [DynamoDBHashKey("PK")]
    public string PK { get; set; } = string.Empty;

    [DynamoDBProperty("Name")]
    public string Name { get; set; } = string.Empty;

    [DynamoDBProperty("Description")]
    public string Description { get; set; } = string.Empty;

    [DynamoDBProperty("IsActive")]
    public bool IsActive { get; set; }

    [DynamoDBProperty("CreatedAt")]
    public string CreatedAt { get; set; } = string.Empty;

    [DynamoDBProperty("UpdatedAt")]
    public string? UpdatedAt { get; set; }

    public SampleEntity ToDomain()
    {
        // Mapeamento simplificado — ajuste conforme necessário
        SampleEntity sample = SampleEntity.Create(Name, Description);
        return sample;
    }

    public static SampleDynamoDocument FromDomain(SampleEntity sample) => new()
    {
        PK = sample.Id.ToString(),
        Name = sample.Name,
        Description = sample.Description,
        IsActive = sample.IsActive,
        CreatedAt = sample.CreatedAt.ToString("O"),
        UpdatedAt = sample.UpdatedAt?.ToString("O")
    };
}
