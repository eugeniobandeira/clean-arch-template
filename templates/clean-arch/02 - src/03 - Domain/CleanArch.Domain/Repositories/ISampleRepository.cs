using CleanArch.Domain.Entities;

namespace CleanArch.Domain.Repositories;

public interface ISampleRepository
{
    Task<Sample?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sample>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Sample sample, CancellationToken cancellationToken = default);
    Task UpdateAsync(Sample sample, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
