using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace CleanArch.Infrastructure.Persistence.DynamoDB;

/// <summary>
/// Contexto para operações com DynamoDB.
/// Use para entidades que se beneficiam de persistência NoSQL:
/// audit logs, eventos, sessões, caches, etc.
/// </summary>
public sealed class DynamoDbContext : IDisposable
{
#pragma warning disable CS0618 // DynamoDBContext(IAmazonDynamoDB) obsolete — migrate to DynamoDBContextBuilder when SDK stabilizes
    private readonly DynamoDBContext _context;
#pragma warning restore CS0618

    public DynamoDbContext(IAmazonDynamoDB client)
    {
#pragma warning disable CS0618
        _context = new DynamoDBContext(client);
#pragma warning restore CS0618
    }

    public DynamoDBContext Context => _context;

    public void Dispose()
    {
        _context.Dispose();
    }
}
