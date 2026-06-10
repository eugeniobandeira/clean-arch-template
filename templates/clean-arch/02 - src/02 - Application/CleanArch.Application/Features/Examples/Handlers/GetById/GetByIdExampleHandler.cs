using CleanArch.Application.Common.Handler;
using CleanArch.Domain.Constants;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Examples.Handlers.GetById;

public sealed class GetByIdExampleHandler(
    IRepository<ExampleEntity> repository,
    ILogger<GetByIdExampleHandler> logger) : IHandler<Guid, ExampleEntity>
{
    public async Task<ErrorOr<ExampleEntity>> Handle(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Fetching example. Id={Id}", id);

        ExampleEntity? entity = await repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
        {
            logger.LogWarning("Example not found. Id={Id}", id);
            return Error.NotFound(ExampleErrorCodes.NotFound, $"Example {id} not found.");
        }

        logger.LogDebug("Example fetched successfully. Response={@Response}", entity);

        return entity;
    }
}
