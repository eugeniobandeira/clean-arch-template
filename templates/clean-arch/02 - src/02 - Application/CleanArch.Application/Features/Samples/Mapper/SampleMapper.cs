using CleanArch.Application.Features.Samples.Handlers.Create.Request;
using CleanArch.Application.Features.Samples.Handlers.Update.Request;
using CleanArch.Domain.Entities;

namespace CleanArch.Application.Features.Samples.Mapper;

public static class SampleMapper
{
    public static SampleEntity CreateSample(CreateSampleRequest request)
        => SampleEntity.Create(request.Name, request.Description);

    public static void UpdateSample(SampleEntity entity, UpdateSampleRequest request)
        => entity.Update(request.Name, request.Description);

    public static SampleResponse ToResponse(SampleEntity entity)
        => new(entity.Id, entity.Name, entity.Description, entity.IsActive, entity.CreatedAt, entity.UpdatedAt);
}
