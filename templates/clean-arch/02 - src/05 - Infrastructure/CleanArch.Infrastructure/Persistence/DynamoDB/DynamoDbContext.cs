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
    private readonly IDynamoDBContext _context;
    private bool _disposed;

    public DynamoDbContext(IAmazonDynamoDB client)
    {
        _context = new DynamoDBContext(client);
    }

    public IDynamoDBContext Context => _context;

    public void Dispose()
    {
        if (!_disposed)
        {
            _context.Dispose();
            _disposed = true;
        }
    }
}
