using CleanArch.Application.Extensions;
using CleanArch.Application.Features.Samples.Mapper;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using CleanArch.Domain.Interfaces.UnitOfWork;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Samples.Handlers.Update;

public sealed class UpdateSampleHandler(
    IGetByIdRepository<SampleEntity> getByIdRepository,
    IUpdateRepository<SampleEntity> updateRepository,
    IValidator<UpdateSampleCommand> validator,
    IUnitOfWork unitOfWork,
    ILogger<UpdateSampleHandler> logger) : IUpdateSampleHandler
{
    public async Task<ErrorOr<SampleEntity>> Handle(
        UpdateSampleCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating sample. Payload: {@Payload}", command);

        List<Error>? errors = await validator.ValidateToErrorsAsync(command, cancellationToken);
        if (errors is not null)
            return errors;

        SampleEntity? entity = await getByIdRepository.GetByIdAsync(command.Id, cancellationToken);

        SampleMapper.UpdateSample(entity!, command.Dto);

        await updateRepository.UpdateAsync(entity!, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Sample updated. Response: {@Response}", entity);

        return entity!;
    }
}
