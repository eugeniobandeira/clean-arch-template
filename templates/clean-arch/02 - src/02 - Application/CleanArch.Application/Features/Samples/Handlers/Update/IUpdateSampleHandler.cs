using CleanArch.Domain.Entities;
using ErrorOr;

namespace CleanArch.Application.Features.Samples.Handlers.Update;

public interface IUpdateSampleHandler
{
    Task<ErrorOr<SampleEntity>> Handle(UpdateSampleCommand command, CancellationToken cancellationToken = default);
}
