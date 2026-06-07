using CleanArch.Application.Common.Commands;
using CleanArch.Application.Common.Handler;
using CleanArch.Application.Extensions;
using CleanArch.Application.Features.Examples.Handlers.Update.Request;
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
    IValidator<UpdateCommand<UpdateExampleRequest>> validator,
    ILogger<UpdateExampleHandler> logger) : IHandler<UpdateCommand<UpdateExampleRequest>, ExampleEntity>
{
    public async Task<ErrorOr<ExampleEntity>> Handle(
        UpdateCommand<UpdateExampleRequest> command,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating example. Payload={@Payload}", command);

        List<Error>? errors = await validator.ValidateToErrorsAsync(command, cancellationToken);
        if (errors is not null)
            return errors;

        ExampleEntity? entity = await getByIdRepository.GetByIdAsync(command.Id, cancellationToken);

        ExampleMapper.UpdateExample(entity!, command.Dto);

        await updateRepository.UpdateAsync(entity!, cancellationToken);

        logger.LogInformation("Example updated successfully. Response={@Response}", entity);

        return entity!;
    }
}
