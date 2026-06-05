using CleanArch.Domain.Interfaces.UnitOfWork;
using CleanArch.Infrastructure.Persistence.EfCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CleanArch.Infrastructure.DataAccess;

public sealed class UnitOfWork(AppDbContext dbContext) : ITransactionUnitOfWork
{
    private IDbContextTransaction? _transaction;

    public async Task CommitAsync(CancellationToken cancellationToken = default)
        => await dbContext.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
        _transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _transaction!.CommitAsync(cancellationToken);
        dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _transaction!.RollbackAsync(cancellationToken);
        dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
    }
}
