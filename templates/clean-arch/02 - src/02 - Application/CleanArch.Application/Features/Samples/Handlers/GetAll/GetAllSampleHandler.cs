using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Filters;
using CleanArch.Domain.Interfaces.Common;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Samples.Handlers.GetAll;

public sealed class GetAllSampleHandler(
    IGetAllRepository<SampleEntity, SampleFilter> repository,
    ILogger<GetAllSampleHandler> logger) : IGetAllSampleHandler
{
    public async Task<ErrorOr<PagedResult<SampleEntity>>> Handle(SampleFilter filter, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching all active samples. Page={Page}, PageSize={PageSize}.", filter.Page, filter.PageSize);

        PagedResult<SampleEntity> result = await repository.GetAllAsync(filter with { IsActive = true }, cancellationToken);

        logger.LogDebug("Fetched {Count} of {Total} active samples.", result.Items.Count, result.Total);

        return ErrorOrFactory.From(result);
    }
}
