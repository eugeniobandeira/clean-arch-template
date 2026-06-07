using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Filters;
using CleanArch.Domain.Interfaces.Common;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Examples.Handlers.GetAll;

public sealed class GetAllExampleHandler(
    IGetAllRepository<ExampleEntity, ExampleFilter> repository,
    ILogger<GetAllExampleHandler> logger) : IGetAllExampleHandler
{
    public async Task<ErrorOr<PagedResult<ExampleEntity>>> Handle(ExampleFilter filter, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching all active examples. Page={Page}, PageSize={PageSize}.", filter.Page, filter.PageSize);

        PagedResult<ExampleEntity> result = await repository.GetAllAsync(filter with { IsActive = true }, cancellationToken);

        logger.LogDebug("Fetched {Count} of {Total} active examples.", result.Items.Count, result.Total);

        return ErrorOrFactory.From(result);
    }
}
