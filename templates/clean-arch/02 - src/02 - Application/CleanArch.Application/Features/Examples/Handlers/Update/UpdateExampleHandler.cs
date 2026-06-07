using CleanArch.Application.Extensions;
using CleanArch.Application.Features.Examples.Mapper;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Examples.Handlers.Update;

public sealed class UpdateExampleHandler(
    IGetByIdRepository<ExampleEntity> getByIdRepository,
    IUpdateRepository<ExampleEntity> updateRepository,
    IValidator<UpdateExampleCommand> validator,
    ILogger<UpdateExampleHandler> logger) : IUpdateExampleHandler
{
    public async Task<ErrorOr<ExampleEntity>> Handle(
        UpdateExampleCommand command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating example. Payload: {@Payload}", command);

        List<Error>? errors = await validator.ValidateToErrorsAsync(command, cancellationToken);
        if (errors is not null)
            return errors;

        ExampleEntity? entity = await getByIdRepository.GetByIdAsync(command.Id, cancellationToken);

        ExampleMapper.UpdateExample(entity!, command.Dto);

        await updateRepository.UpdateAsync(entity!, cancellationToken);

        logger.LogInformation("Example updated. Response: {@Response}", entity);

        return entity!;
    }
}
