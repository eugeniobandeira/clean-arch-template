using CleanArch.Domain.Constants;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Examples.Handlers.GetById;

public sealed class GetByIdExampleHandler(
    IGetByIdRepository<ExampleEntity> repository,
    ILogger<GetByIdExampleHandler> logger) : IGetByIdExampleHandler
{
    public async Task<ErrorOr<ExampleEntity>> Handle(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching example. Id: {ExampleId}", id);

        ExampleEntity? entity = await repository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
            return Error.NotFound(ExampleErrorCodes.NotFound, $"Example {id} not found.");

        logger.LogInformation("Example fetched. Response: {@Response}", entity);

        return entity;
    }
}
