using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Filters;
using ErrorOr;

namespace CleanArch.Application.Features.Samples.Handlers.GetAll;

public interface IGetAllSampleHandler
{
    Task<ErrorOr<PagedResult<SampleEntity>>> Handle(SampleFilter filter, CancellationToken cancellationToken = default);
}
