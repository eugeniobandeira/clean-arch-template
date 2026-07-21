using CleanArch.Application.Common.Handler;
using CleanArch.Application.Extensions;
using CleanArch.Application.Features.Examples.Handlers.GetAll.Request;
using CleanArch.Application.Features.Examples.Mapper;
using CleanArch.Domain.Common;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Examples;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Examples.Handlers.GetAll;

public sealed class GetAllExampleHandler(
    IExampleRepository repository,
    IValidator<GetAllExampleRequest> validator,
    ILogger<GetAllExampleHandler> logger) : IHandler<GetAllExampleRequest, PagedResult<ExampleEntity>>
{
    public async Task<ErrorOr<PagedResult<ExampleEntity>>> Handle(GetAllExampleRequest request, CancellationToken ct = default)
    {
        logger.LogDebug("Fetching examples. Payload={@Payload}", request);

        List<Error>? errors = await validator.ValidateToErrorsAsync(request, ct);
        if (errors is not null)
            return errors;

        PagedResult<ExampleEntity> result = await repository.GetAllAsync(ExampleMapper.ToFilter(request), ct);

        logger.LogDebug("Fetched {Count} of {Total} examples.", result.Items.Count, result.Total);

        return ErrorOrFactory.From(result);
    }
}
