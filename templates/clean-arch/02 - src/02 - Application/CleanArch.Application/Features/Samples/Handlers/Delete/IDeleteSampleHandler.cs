using ErrorOr;

namespace CleanArch.Application.Features.Samples.Handlers.Delete;

public interface IDeleteSampleHandler
{
    Task<ErrorOr<Deleted>> Handle(Guid id, CancellationToken cancellationToken = default);
}
