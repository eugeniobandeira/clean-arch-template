using CleanArch.Application.Features.Examples.Handlers.GetAll.Request;
using CleanArch.Domain.Interfaces.Common;
using CleanArch.Domain.Interfaces.Examples;
using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Repositories;

public sealed class ExampleRepository(AppDbContext context) : IExampleRepository, IGetAllRepository<ExampleEntity, GetAllExampleRequest>
{
    public async Task AddAsync(ExampleEntity entity, CancellationToken cancellationToken = default)
        => await context.Examples.AddAsync(entity, cancellationToken);

    public async Task<ExampleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Examples.FindAsync([id], cancellationToken);

    public async Task<PagedResult<ExampleEntity>> GetAllAsync(
        GetAllExampleRequest request,
        CancellationToken cancellationToken = default)
    {
        IQueryable<ExampleEntity> query = context.Examples.AsNoTracking();

        if (request.Name is not null)
            query = query.Where(e => e.Name.Contains(request.Name));

        if (request.IsActive is not null)
            query = query.Where(e => e.IsActive == request.IsActive);

        int total = await query.CountAsync(cancellationToken);

        List<ExampleEntity> items = await query
            .OrderBy(e => e.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
