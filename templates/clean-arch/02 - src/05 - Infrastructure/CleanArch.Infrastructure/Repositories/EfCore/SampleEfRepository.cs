using CleanArch.Domain.Entities;
using CleanArch.Domain.Repositories;
using CleanArch.Infrastructure.Persistence.EfCore;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Repositories.EfCore;

public sealed class SampleEfRepository(AppDbContext dbContext) : ISampleRepository
{
    public async Task<Sample?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Samples
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<Sample>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Samples
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Sample sample, CancellationToken cancellationToken = default)
    {
        await dbContext.Samples.AddAsync(sample, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Sample sample, CancellationToken cancellationToken = default)
    {
        dbContext.Samples.Update(sample);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sample = await dbContext.Samples.FindAsync([id], cancellationToken);
        if (sample is not null)
        {
            dbContext.Samples.Remove(sample);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
