using CleanArch.Domain.Constants;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using CleanArch.Domain.Interfaces.UnitOfWork;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Samples.Handlers.Delete;

public sealed class DeleteSampleHandler(
    IGetByIdRepository<SampleEntity> getByIdRepository,
    IDeleteRepository<SampleEntity> deleteRepository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteSampleHandler> logger) : IDeleteSampleHandler
{
    public async Task<ErrorOr<Deleted>> Handle(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting sample. Id: {SampleId}", id);

        SampleEntity? entity = await getByIdRepository.GetByIdAsync(id, cancellationToken);

        if (entity is null)
            return Error.NotFound(SampleErrorCodes.NotFound, $"Sample {id} not found.");

        await deleteRepository.DeleteAsync(entity, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Sample {SampleId} deleted.", id);

        return Result.Deleted;
    }
}
