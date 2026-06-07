using CleanArch.Domain.Constants;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Examples.Handlers.Delete;

public sealed class DeleteExampleHandler(
    IGetByIdRepository<ExampleEntity> getByIdRepository,
    IDeleteRepository<ExampleEntity> deleteRepository,
    ILogger<DeleteExampleHandler> logger) : IDeleteExampleHandler
{
    public async Task<ErrorOr<Deleted>> Handle(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting example. Id: {ExampleId}", id);

        ExampleEntity? entity = await getByIdRepository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
            return Error.NotFound(ExampleErrorCodes.NotFound, $"Example {id} not found.");

        await deleteRepository.DeleteAsync(entity, cancellationToken);

        logger.LogInformation("Example {ExampleId} deleted.", id);

        return Result.Deleted;
    }
}
