using CleanArch.Application.Common.Handler;
using CleanArch.Application.Features.Examples.Handlers.GetAll.Request;
using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Examples.Handlers.GetAll;

public sealed class GetAllExampleHandler(
    IRepository<ExampleEntity, GetAllExampleRequest> repository,
    ILogger<GetAllExampleHandler> logger) : IHandler<GetAllExampleRequest, PagedResult<ExampleEntity>>
{
    public async Task<ErrorOr<PagedResult<ExampleEntity>>> Handle(GetAllExampleRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching examples. Payload={@Payload}", request);

        PagedResult<ExampleEntity> result = await repository.GetAllAsync(request, cancellationToken);

        logger.LogDebug("Fetched {Count} of {Total} examples.", result.Items.Count, result.Total);

        return ErrorOrFactory.From(result);
    }
}
