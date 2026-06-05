using CleanArch.Application.Features.Samples.Handlers.Create.Request;
using CleanArch.Domain.Entities;
using ErrorOr;

namespace CleanArch.Application.Features.Samples.Handlers.Create;

public interface ICreateSampleHandler
{
    Task<ErrorOr<SampleEntity>> Handle(CreateSampleRequest request, CancellationToken cancellationToken = default);
}
