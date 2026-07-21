using CleanArch.Application.Common.Handler;
using CleanArch.Application.Extensions;
using CleanArch.Application.Features.Examples.Handlers.Create.Request;
using CleanArch.Application.Features.Examples.Mapper;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces;
using CleanArch.Domain.Interfaces.Common;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Examples.Handlers.Create;

public sealed class CreateExampleHandler(
    IAddRepository<ExampleEntity> repository,
    IUnitOfWork unitOfWork,
    IValidator<CreateExampleRequest> validator,
    ILogger<CreateExampleHandler> logger) : IHandler<CreateExampleRequest, ExampleEntity>
{
    public async Task<ErrorOr<ExampleEntity>> Handle(
        CreateExampleRequest request,
        CancellationToken ct = default)
    {
        logger.LogInformation("Creating example. Payload={@Payload}", request);

        List<Error>? errors = await validator.ValidateToErrorsAsync(request, ct);
        if (errors is not null)
            return errors;

        ExampleEntity entity = ExampleMapper.CreateExample(request);

        await repository.AddAsync(entity, ct);
        await unitOfWork.CommitAsync(ct);

        logger.LogInformation("Example created successfully. Response={@Response}", entity);

        return entity;
    }
}
