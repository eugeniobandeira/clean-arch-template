using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Filters;
using CleanArch.Domain.Interfaces.Common;
using CleanArch.Infrastructure.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Repositories.EfCore;

public sealed class SampleEfRepository(AppDbContext dbContext) :
    IAddRepository<SampleEntity>,
    IGetByIdRepository<SampleEntity>,
    IUpdateRepository<SampleEntity>,
    IGetAllRepository<SampleEntity, SampleFilter>,
    IDeleteRepository<SampleEntity>
{
    public async Task AddAsync(SampleEntity entity, CancellationToken cancellationToken = default)
        => await dbContext.Samples.AddAsync(entity, cancellationToken);

    public async Task<SampleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Samples
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PagedResult<SampleEntity>> GetAllAsync(SampleFilter? filter = null, CancellationToken cancellationToken = default)
    {
        IQueryable<SampleEntity> query = dbContext.Samples.AsNoTracking();

        if (filter?.IsActive.HasValue is true)
            query = query.Where(x => x.IsActive == filter.IsActive.Value);

        int total = await query.CountAsync(cancellationToken);

        int page = filter?.Page ?? 1;
        int pageSize = filter?.PageSize ?? 10;

        List<SampleEntity> items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<SampleEntity>(items, total);
    }

    public Task UpdateAsync(SampleEntity entity, CancellationToken cancellationToken = default)
    {
        dbContext.Samples.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(SampleEntity entity, CancellationToken cancellationToken = default)
    {
        dbContext.Samples.Remove(entity);
        return Task.CompletedTask;
    }
}
