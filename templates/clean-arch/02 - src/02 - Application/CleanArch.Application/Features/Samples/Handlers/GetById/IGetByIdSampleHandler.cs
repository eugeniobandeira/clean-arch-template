using CleanArch.Domain.Entities;
using ErrorOr;

namespace CleanArch.Application.Features.Samples.Handlers.GetById;

public interface IGetByIdSampleHandler
{
    Task<ErrorOr<SampleEntity>> Handle(Guid id, CancellationToken cancellationToken = default);
}
