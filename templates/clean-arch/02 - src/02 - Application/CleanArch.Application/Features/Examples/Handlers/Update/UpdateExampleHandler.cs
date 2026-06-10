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
    IRepository<ExampleEntity> repository,
    IValidator<UpdateExampleRequest> validator,
    ILogger<UpdateExampleHandler> logger) : IHandler<UpdateExampleRequest, ExampleEntity>
{
    public async Task<ErrorOr<ExampleEntity>> Handle(
        UpdateExampleRequest request,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating example. Payload={@Payload}", request);

        List<Error>? errors = await validator.ValidateToErrorsAsync(request, cancellationToken);
        if (errors is not null)
            return errors;

        ExampleEntity? entity = await repository.GetByIdAsync(request.Id, cancellationToken);

        ExampleMapper.UpdateExample(entity!, request);

        await repository.UpdateAsync(entity!, cancellationToken);

        logger.LogInformation("Example updated successfully. Response={@Response}", entity);

        return entity!;
    }
}
