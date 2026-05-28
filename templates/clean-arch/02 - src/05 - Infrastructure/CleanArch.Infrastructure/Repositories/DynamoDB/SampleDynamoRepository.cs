using Amazon.DynamoDBv2.DataModel;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Repositories;
using CleanArch.Infrastructure.Persistence.DynamoDB;

namespace CleanArch.Infrastructure.Repositories.DynamoDB;

/// <summary>
/// Implementação alternativa do repositório usando DynamoDB.
/// Troque o registro no DI para usar esta implementação.
/// </summary>
public sealed class SampleDynamoRepository(DynamoDbContext dynamoContext) : ISampleRepository
{
    public async Task<Sample?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await dynamoContext.Context.LoadAsync<SampleDynamoDocument>(
            id.ToString(), cancellationToken);

        return item is null ? null : item.ToDomain();
    }

    public async Task<IEnumerable<Sample>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var conditions = new List<ScanCondition>();
        var search = dynamoContext.Context.ScanAsync<SampleDynamoDocument>(conditions);
        var results = await search.GetRemainingAsync(cancellationToken);
        return results.Select(x => x.ToDomain());
    }

    public async Task AddAsync(Sample sample, CancellationToken cancellationToken = default)
    {
        var document = SampleDynamoDocument.FromDomain(sample);
        await dynamoContext.Context.SaveAsync(document, cancellationToken);
    }

    public async Task UpdateAsync(Sample sample, CancellationToken cancellationToken = default)
    {
        var document = SampleDynamoDocument.FromDomain(sample);
        await dynamoContext.Context.SaveAsync(document, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await dynamoContext.Context.DeleteAsync<SampleDynamoDocument>(
            id.ToString(), cancellationToken);
    }
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

    public Sample ToDomain()
    {
        // Mapeamento simplificado — ajuste conforme necessário
        var sample = Sample.Create(Name, Description);
        return sample;
    }

    public static SampleDynamoDocument FromDomain(Sample sample) => new()
    {
        PK = sample.Id.ToString(),
        Name = sample.Name,
        Description = sample.Description,
        IsActive = sample.IsActive,
        CreatedAt = sample.CreatedAt.ToString("O"),
        UpdatedAt = sample.UpdatedAt?.ToString("O")
    };
}
