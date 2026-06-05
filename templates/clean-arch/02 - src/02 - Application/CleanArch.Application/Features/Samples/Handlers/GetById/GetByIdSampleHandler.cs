using CleanArch.Domain.Constants;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Samples.Handlers.GetById;

public sealed class GetByIdSampleHandler(
    IGetByIdRepository<SampleEntity> repository,
    ILogger<GetByIdSampleHandler> logger) : IGetByIdSampleHandler
{
    public async Task<ErrorOr<SampleEntity>> Handle(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching sample. Id: {SampleId}", id);

        SampleEntity? entity = await repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
            return Error.NotFound(SampleErrorCodes.NotFound, $"Sample {id} not found.");

        logger.LogInformation("Sample fetched. Response: {@Response}", entity);

        return entity;
    }
}
