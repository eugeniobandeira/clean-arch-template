using CleanArch.Domain.Common;

namespace CleanArch.Domain.Interfaces.Common;

public interface IGetAllRepository<T, TFilter>
    where T : class
    where TFilter : class
{
    Task<PagedResult<T>> GetAllAsync(TFilter? filter = null, CancellationToken cancellationToken = default);
}
