using CleanArch.Application.Extensions;
using CleanArch.Application.Features.Samples.Handlers.Create.Request;
using CleanArch.Application.Features.Samples.Mapper;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces.Common;
using CleanArch.Domain.Interfaces.UnitOfWork;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Features.Samples.Handlers.Create;

public sealed class CreateSampleHandler(
    IAddRepository<SampleEntity> repository,
    IValidator<CreateSampleRequest> validator,
    IUnitOfWork unitOfWork,
    ILogger<CreateSampleHandler> logger) : ICreateSampleHandler
{
    public async Task<ErrorOr<SampleEntity>> Handle(
        CreateSampleRequest request,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating sample. Payload: {@Payload}", request);

        List<Error>? errors = await validator.ValidateToErrorsAsync(request, cancellationToken);
        if (errors is not null)
            return errors;

        SampleEntity entity = SampleMapper.CreateSample(request);

        await repository.AddAsync(entity, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation("Sample created. Response: {@Response}", entity);

        return entity;
    }
}
