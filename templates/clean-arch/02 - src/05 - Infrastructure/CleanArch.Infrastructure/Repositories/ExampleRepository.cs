using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Filters;
using CleanArch.Domain.Interfaces.Common;

namespace CleanArch.Infrastructure.Repositories;

public sealed class ExampleRepository :
    IAddRepository<ExampleEntity>,
    IGetByIdRepository<ExampleEntity>,
    IUpdateRepository<ExampleEntity>,
    IDeleteRepository<ExampleEntity>,
    IGetAllRepository<ExampleEntity, ExampleFilter>
{
    public Task AddAsync(ExampleEntity entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ExampleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(ExampleEntity entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(ExampleEntity entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<ExampleEntity>> GetAllAsync(ExampleFilter? filter = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
