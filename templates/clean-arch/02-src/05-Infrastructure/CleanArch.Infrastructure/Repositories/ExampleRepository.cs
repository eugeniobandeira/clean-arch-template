using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Filters.Examples;
using CleanArch.Domain.Interfaces.Examples;
using CleanArch.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Repositories;

public sealed class ExampleRepository(AppDbContext context) : IExampleRepository
{
    public async Task AddAsync(ExampleEntity entity, CancellationToken cancellationToken = default)
        => await context.Examples.AddAsync(entity, cancellationToken);

    public async Task<ExampleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Examples.FindAsync([id], cancellationToken);

    public async Task<PagedResult<ExampleEntity>> GetAllAsync(
        ExampleFilter filter,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ExampleEntity> query = context.Examples.AsNoTracking();

        if (filter.Name is not null)
            query = query.Where(e => e.Name.Contains(filter.Name));

        if (filter.IsActive is not null)
            query = query.Where(e => e.IsActive == filter.IsActive);

        int total = await query.CountAsync(cancellationToken);

        List<ExampleEntity> items = await query
            .OrderBy(e => e.Name)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<ExampleEntity>(items, total);
    }

    public Task UpdateAsync(ExampleEntity entity, CancellationToken cancellationToken = default)
    {
        context.Examples.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ExampleEntity entity, CancellationToken cancellationToken = default)
    {
        context.Examples.Remove(entity);
        return Task.CompletedTask;
    }
}
